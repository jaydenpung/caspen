; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!
#define MyAppName "Caspen"
#define MyAppExeName "Caspen.exe"

[Setup]
AppName=Caspen
AppVersion=1.0
AppPublisher=Jay Pung
AppPublisherURL=https://www.JayPung.com
DefaultDirName={pf}\Caspen
DefaultGroupName=Caspen
UninstallDisplayIcon={app}\Caspen.exe
Compression=lzma2
SolidCompression=yes
OutputDir=.
OutputBaseFilename=CaspenSetup
PrivilegesRequired=admin
DisableDirPage=no

[Files]
Source: "Caspen.exe"; DestDir: "{app}"; DestName: {#MyAppExeName}; Flags: ignoreversion
Source: "Ookii.Dialogs.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion                     
Source: "Ookii.Dialogs.Wpf.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "icon.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Caspen"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\icon.ico"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; IconFilename: "{app}\icon.ico"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Registry]
Root: "HKCU"; Subkey: "SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"; ValueType: String; ValueName: "{app}\{#MyAppExeName}"; ValueData: "RUNASADMIN"; Flags: uninsdeletekeyifempty uninsdeletevalue

[Run]
Filename: {app}\{#MyAppExeName}; Description: Launch Now; Flags: postinstall skipifsilent nowait runascurrentuser

[InstallDelete]
Type: filesandordirs; Name: "{app}\app.publish"