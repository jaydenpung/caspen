using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Caspen.Model;
using System.IO.Compression;

namespace Caspen
{
    public partial class Form1 : Form
    {
        List<string> subtitleFiles, videoFiles;
        int subtitleFileEpisodeIndex, subtitleFileEpisodeLength;
        int videoFileEpisodeIndex, videoFileEpisodeLength;

        public Form1()
        {
            InitializeComponent();

            //Initialize events
            tbVideoFileName.MouseUp += tbVideoFileName_MouseUp;
            tbSubtitleFileName.MouseUp += tbSubtitleFileName_MouseUp;
        }

        //Get episode index of video files
        private void tbVideoFileName_MouseUp(object sender, MouseEventArgs e)
        {
            if (tbVideoFileName.SelectedText != "")
            {
                lblVideoEpisode.Text = tbVideoFileName.SelectedText;
                videoFileEpisodeIndex = tbVideoFileName.SelectionStart;
                videoFileEpisodeLength = tbVideoFileName.SelectionLength;
            }
            else
            {
                lblVideoEpisode.Text = "N/A";
                videoFileEpisodeIndex = 0;
                videoFileEpisodeLength = 0;
            }
        }

        //Get episode index of subtitle files
        private void tbSubtitleFileName_MouseUp(object sender, MouseEventArgs e)
        {
            if (tbSubtitleFileName.SelectedText != "")
            {
                lblSubtitleEpisode.Text = tbSubtitleFileName.SelectedText;
                subtitleFileEpisodeIndex = tbSubtitleFileName.SelectionStart;
                subtitleFileEpisodeLength = tbSubtitleFileName.SelectionLength;
            }
            else
            {
                lblSubtitleEpisode.Text = "N/A";
                subtitleFileEpisodeIndex = 0;
                subtitleFileEpisodeLength = 0;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About frmAbout = new About();
            frmAbout.StartPosition = FormStartPosition.CenterParent;
            frmAbout.ShowDialog();
        }

        // Choose directory
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderSelector = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (folderSelector.ShowDialog() == true)
            {
                reset();

                //Handle archived subtitle files
                var archiveExtensions = new List<string> { ".zip", ".7z", ".rar" };
                var archiveFiles = Directory.GetFiles(folderSelector.SelectedPath, "*", SearchOption.AllDirectories)
                    .Where(s => archiveExtensions.Contains(Path.GetExtension(s))).ToList<string>();
                handleArchivedFiles(archiveFiles);

                var videoExtensions = new List<string> { ".avi", ".wmv", ".mp4", ".mov", ".mkv", ".webm", ".mpeg", ".m4v" };
                subtitleFiles = Directory.GetFiles(folderSelector.SelectedPath, "*.srt", SearchOption.AllDirectories)
                    .OrderBy(filename => filename).ToList<string>();
                videoFiles = Directory.GetFiles(folderSelector.SelectedPath, "*", SearchOption.AllDirectories)
                    .Where(s => videoExtensions.Contains(Path.GetExtension(s)))
                    .OrderBy(filename => filename).ToList<string>();


                tbVideoFileName.Text = videoFiles.First();
                tbSubtitleFileName.Text = subtitleFiles.First();
                tbSeriesName.Text = Path.GetFileName(folderSelector.SelectedPath);
            }
        }

        private void handleArchivedFiles(List<string> archivedFiles)
        {
            foreach (var archivedFile in archivedFiles)
            {
                string zipPath = archivedFile;
                string extractPath = Path.GetDirectoryName(archivedFile);

                // Normalizes the path.
                extractPath = Path.GetFullPath(extractPath);

                // Ensures that the last character on the extraction path
                // is the directory separator char. 
                // Without this, a malicious zip file could try to traverse outside of the expected
                // extraction path.
                if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    extractPath += Path.DirectorySeparatorChar;

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase))
                        {
                            // Gets the full path to ensure that relative segments are removed.
                            string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                            // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                            // are case-insensitive.
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal) && !File.Exists(destinationPath))
                            {
                                entry.ExtractToFile(destinationPath);
                            }
                        }
                    }
                }
            };
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            processFiles(videoFiles, videoFileEpisodeIndex, videoFileEpisodeLength);
            processFiles(subtitleFiles, subtitleFileEpisodeIndex, subtitleFileEpisodeLength);

            SuccessForm frmSUccess = new SuccessForm();
            frmSUccess.StartPosition = FormStartPosition.CenterParent;
            frmSUccess.ShowDialog();
        }   

        private void processFiles(List<string> files, int index, int length)
        {
            var customFiles = files.Select<string, CustomFile>(file => new CustomFile(file, file.Substring(index, length))).GroupBy(x => x.episode).Select(x => x.First()).ToList<CustomFile>();

            try
            {
                customFiles.ForEach(customFile =>
                    File.Move(customFile.filePath, Path.GetDirectoryName(customFile.filePath) + "\\" + tbSeriesName.Text + " - " + customFile.episode + Path.GetExtension(customFile.filePath))
                );
            }
            catch (Exception ex)
            {
            }
        }

        private void reset()
        {
            videoFileEpisodeIndex = 0;
            videoFileEpisodeLength = 0;
            subtitleFileEpisodeIndex = 0;
            subtitleFileEpisodeLength = 0;
            lblVideoEpisode.Text = "N/A";
            lblSubtitleEpisode.Text = "N/A";
            tbVideoFileName.Text = "";
            tbSubtitleFileName.Text = "";
        }
    }
}
