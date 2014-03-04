#define MyAppName "Switcheroo"
#define MyAppPublisher "Regin Larsen"
#define MyAppURL "http://www.switcheroo.io"
#define MyAppExeName "switcheroo.exe"
#define MyAppPath SourcePath + "Source"
#define MyAppVer = GetFileVersion(MyAppPath + "\switcheroo.exe")

[Setup]
AppId={{A5AF4C34-70A7-4D3B-BA18-E49C0AEEA5E6}
AppMutex=DBDE24E4-91F6-11DF-B495-C536DFD72085-switcheroo
AppName={#MyAppName}
AppVerName={#MyAppName} v{#MyAppVer}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: english; MessagesFile: compiler:Default.isl

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
Name: startupfolder; Description: Startup with Windows

[Files]
Source: {#MyAppPath}\switcheroo.exe; DestDir: {app}; Flags: ignoreversion
Source: {#MyAppPath}\*.dll; DestDir: {app}; Flags: ignoreversion
Source: {#MyAppPath}\switcheroo.exe.config; DestDir: {app}; Flags: ignoreversion
Source: {#MyAppPath}\LICENSE.txt; DestDir: {app}; Flags: ignoreversion

[Icons]
Name: {group}\{#MyAppName}; Filename: {app}\{#MyAppExeName}
Name: {group}\{cm:UninstallProgram,{#MyAppName}}; Filename: {uninstallexe}
Name: {commondesktop}\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: desktopicon
Name: {commonstartup}\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: startupfolder

[Run]
Filename: {app}\{#MyAppExeName}; Description: {cm:LaunchProgram,{#MyAppName}}; Flags: nowait postinstall skipifsilent

[Code]
const

// The following was stolen from the Witty Twitter installer.
// http://code.google.com/p/wittytwitter/source/browse/trunk/Witty/Installer/Installer.iss

dotnetRedistURL = 'http://www.microsoft.com/en-us/download/details.aspx?id=30653';
dotnetRegKey = 'SOFTWARE\Microsoft\Net Framework Setup\NDP\v4.0';
version = '4.5';

function InitializeSetup(): Boolean;
var
    ErrorCode: Integer;
    NetFrameWorkInstalled : Boolean;
    InstallDotNetResponse : Boolean;
begin
	NetFrameWorkInstalled := RegKeyExists(HKLM,dotnetRegKey);
	if NetFrameWorkInstalled =true then
	   begin
		  Result := true;
	   end
	else
	   begin
		  InstallDotNetResponse := MsgBox('This setup requires version ' + version + ' of the .NET Framework. Please download and install the .NET Framework and run this setup again. Do you want to download the framework now?',mbConfirmation,MB_YESNO)= idYes;
		  if InstallDotNetResponse =false then
			begin
			  Result:=false;
			end
		  else
			begin
			  Result:=false;
			  ShellExec('open',dotnetRedistURL,'','',SW_SHOWNORMAL,ewNoWait,ErrorCode);
			end;
	   end;
	end;
