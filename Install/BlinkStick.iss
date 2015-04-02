#define use_dotnetfx40

#define AppName "BlinkStick Client"
#define MyGetVersion() ParseVersion(AddBackslash(SourcePath) + "..\BlinkStick\bin\Release\BlinkStick.exe", Local[0], Local[1], Local[2], Local[3]), Str(Local[0]) + "." + Str(Local[1]) + "." + Str(Local[2]);
#define AppVersion GetFileVersion(AddBackslash(SourcePath) + "..\BlinkStick\bin\Release\BlinkStick.exe")
#define AppPublisher "Agile Innovative Ltd"
#define AppURL "http://www.blinkstick.com/"
#define AppExeName "BlinkStick.exe"

[CustomMessages]
win_sp_title=Windows %1 Service Pack %2

[Setup]
AppId={{1C20C67E-1414-49A9-8A5C-2409A420A26E}
AppName={#AppName}
AppVersion={#MyGetVersion()}
AppVerName={#AppName} {#MyGetVersion()}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=setup
OutputBaseFilename=BlinkStick_Setup_{#MyGetVersion()}
Compression=lzma
SolidCompression=yes
ShowLanguageDialog=no
AlwaysShowGroupOnReadyPage=True
AlwaysShowDirOnReadyPage=True
AppCopyright=Agile Innovative Ltd
UninstallDisplayName=Uninstall BlinkStick
UninstallDisplayIcon={app}\icon.ico
MinVersion=0,5.01sp3
LicenseFile=..\LICENSE.txt

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\BlinkStick\bin\Release\BlinkStick.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\BlinkStick.Bayeux.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\BlinkStick.Hid.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\HidSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\icon.png"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\logo.png"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\LibUsbDotNet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\log4net.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStick\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: dep\gtk-sharp-2.12.25.msi; DestDir: "{tmp}"; Check: IsGtkNotInstalled; AfterInstall: InstallGtkSharp

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#AppName}}"; Filename: "{#AppURL}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#AppExeName}"; Flags: nowait postinstall skipifsilent; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[Registry]
Root: HKLM; Subkey: SOFTWARE\Agile Innovative Ltd\BlinkStick; ValueType: string; ValueName: InstallDir; ValueData: {app}; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: HKLM; Subkey: SOFTWARE\Agile Innovative Ltd\BlinkStick; ValueType: string; ValueName: Version; ValueData: {#AppVersion}; Flags: uninsdeletevalue

#include "scripts\products.iss"

#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"

#ifdef use_dotnetfx40
#include "scripts\products\dotnetfx40client.iss"
#include "scripts\products\dotnetfx40full.iss"
#endif

[Code]

var
  RequireRestart: Boolean;

Function IsGtkNotInstalled() : Boolean;
begin
  Result := Not RegKeyExists(HKLM, 'SOFTWARE\Xamarin\GtkSharp\InstallFolder');
end;

procedure InstallGtkSharp;
var
  ResultCode: Integer;
begin
  if not Exec('msiexec.exe', ExpandConstant('/i "{tmp}\gtk-sharp-2.12.25.msi" /PASSIVE /NORESTART'), '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
  begin
    // you can interact with the user that the installation failed
    MsgBox('GtkSharp installation failed with code: ' + IntToStr(ResultCode) + '.',
      mbError, MB_OK);
  end;
end;

function InitializeSetup(): Boolean;
begin
  RequireRestart := IsGtkNotInstalled;

	//init windows version
	initwinversion();
	
	if not minwinspversion(5, 1, 3) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('winxpsp3_title')]), mbError, MB_OK);
		exit;
	end;

#ifdef use_dotnetfx40
	if (not netfxinstalled(NetFx40Full, '')) then
  begin
		RequireRestart := true;
    dotnetfx40full();
  end;
#endif
  
  Result := true; 
end;

function NeedRestart(): Boolean;
begin
  if (delayedReboot Or RequireRestart) then
		Result := true;
end;

