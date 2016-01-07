#define use_dotnetfx40

#expr Exec(AddBackslash(SourcePath) + "..\BlinkStickClient\bin\Release\BlinkStickClient.exe", "--build-config """ + AddBackslash(SourcePath) + "version.iss""")
#include AddBackslash(SourcePath) + "version.iss"

#define AppName "BlinkStick Client"
#define AppPublisher "Agile Innovative Ltd"
#define AppURL "http://www.blinkstick.com/"
#define AppExeName "BlinkStickClient.exe"

[CustomMessages]
win_sp_title=Windows %1 Service Pack %2

[Setup]
AppId={{1C20C67E-1414-49A9-8A5C-2409A420A26E}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=setup
OutputBaseFilename=BlinkStickClient-Setup-{#AppVersion}-x86
Compression=lzma
SolidCompression=yes
ShowLanguageDialog=no
AlwaysShowGroupOnReadyPage=True
AlwaysShowDirOnReadyPage=True
AppCopyright=Agile Innovative Ltd
UninstallDisplayName=Uninstall BlinkStick Client
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
Source: "dep\gtk-sharp-2.12.25.msi"; DestDir: "{tmp}"; Check: GtkNeedsInstallOrUpgrade; AfterInstall: InstallGtkSharp
Source: "theme\*"; DestDir: "{app}\Theme"; Flags: ignoreversion recursesubdirs
Source: "gtk-libs\*"; DestDir: "{code:GtkInstallDir}"; Flags: ignoreversion onlyifdoesntexist uninsneveruninstall recursesubdirs
Source: "..\BlinkStickClient\bin\Release\*.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStickClient\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStickClient\bin\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStickClient\bin\Release\icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStickClient\bin\Release\icon.png"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BlinkStickClient\bin\Release\logo.png"; DestDir: "{app}"; Flags: ignoreversion
; dll used to check running notepad at install time
Source: "util\psvince.dll"; flags: dontcopy
;psvince is installed in {app} folder, so it will be
;loaded at uninstall time ;to check if client is running
Source: "util\psvince.dll"; DestDir: {app}

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
Root: HKLM; Subkey: SOFTWARE\Agile Innovative Ltd\BlinkStick; ValueType: string; ValueName: VersionName; ValueData: {#AppVersion}; Flags: uninsdeletevalue
Root: HKLM; Subkey: SOFTWARE\Agile Innovative Ltd\BlinkStick; ValueType: string; ValueName: Version; ValueData: {#AppFullVersion}; Flags: uninsdeletevalue

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
// function IsModuleLoaded to call at install time
// added also setuponly flag
function IsModuleLoadedI(modulename: String ):  Boolean;
external 'IsModuleLoaded@files:psvince.dll stdcall setuponly';

// function IsModuleLoadedU to call at uninstall time
// added also uninstallonly flag
function IsModuleLoadedU(modulename: String ):  Boolean;
external 'IsModuleLoaded@{app}\psvince.dll stdcall uninstallonly' ;

var
  RequireRestart: Boolean;

// Procedure to split a string into an array of integers 
procedure Explode(var Dest: TArrayOfInteger; Text: String; Separator: String);
var
  i, p: Integer;
begin
  i := 0;
  repeat
    SetArrayLength(Dest, i+1);
    p := Pos(Separator,Text);
    if p > 0 then begin
      Dest[i] := StrToInt(Copy(Text, 1, p-1));
      Text := Copy(Text, p + Length(Separator), Length(Text));
      i := i + 1;
    end else begin
      Dest[i] := StrToInt(Text);
      Text := '';
    end;
  until Length(Text)=0;
end;

// Function compares version strings numerically:
//     * when v1 = v2, result = 0  
//     * when v1 < v2, result = -1  
//     * when v1 > v2, result = 1
//
// Supports version numbers with trailing zeroes, for example 1.02.05.
// Supports comparison of two version number of different lengths, for example
//     CompareVersions('1.2', '2.0.3')
// When any of the parameters is '' (empty string) it considers version number as 0
function CompareVersions(v1: String; v2: String): Integer;
var
  v1parts: TArrayOfInteger;
  v2parts: TArrayOfInteger;
  i: Integer;
begin
  if v1 = '' then
  begin
    v1 := '0';
  end;

  if v2 = '' then
  begin
    v2 := '0';
  end;

  Explode(v1parts, v1, '.');
  Explode(v2parts, v2, '.');
  
  if (GetArrayLength(v1parts) > GetArrayLength(v2parts)) then
  begin
    SetArrayLength(v2parts, GetArrayLength(v1parts)) 
  end else if (GetArrayLength(v2parts) > GetArrayLength(v1parts)) then
  begin
    SetArrayLength(v1parts, GetArrayLength(v2parts)) 
  end; 
  
  for i := 0 to GetArrayLength(v1parts) - 1 do 
  begin
    if v1parts[i] > v2parts[i] then
    begin
      { v1 is greater }
      Result := 1;
      exit;
    end else if v1parts[i] < v2parts[i] then
    begin
      { v2 is greater }
      Result := -1;
      exit;
    end;
  end;
  
  { Are Equal }
  Result := 0;
end;

{ Debug code
procedure TestVersions(v1: String; v2: String);
begin
  Log(v1 + ' : ' + v2 + ' = ' + IntToStr(CompareVersions(v1, v2)));
end;
{ }

Function GtkNeedsInstallOrUpgrade() : Boolean;
var
  gtkVersion: String;
begin
  Result := Not RegKeyExists(HKLM, 'SOFTWARE\Xamarin\GtkSharp\InstallFolder');
  
  // If Gtk is installed, check the version number to upgrade if necessary
  if not Result then
  begin
    gtkVersion := '';
    
    RegQueryStringValue(HKLM, 'SOFTWARE\Xamarin\GtkSharp\Version', '', gtkVersion);

    Result := CompareVersions(gtkVersion, '2.12.25') = -1;
  end;
  
  if (Result) then begin
    Log('Gtk-Sharp needs upgrade');    
  end else begin
    Log('Gtk-Sharp ' + gtkVersion + ' does not need upgrade');    
  end;
end;

Function GtkInstallDir(param: String) : String;
var
  dir: String;
begin
  
  RegQueryStringValue(HKLM, 'SOFTWARE\Xamarin\GtkSharp\InstallFolder', '', dir);

  Result := dir;
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
  // check if client is running
  if IsModuleLoadedI( 'BlinkStickClient.exe' ) Or IsModuleLoadedI( 'BlinkStick.exe' ) then
  begin
    MsgBox( 'BlinkStick Client is running, please close it and run again setup.', mbError, MB_OK );
    Result := false;
  end
  else 
  begin
    RequireRestart := GtkNeedsInstallOrUpgrade;
{ Debug code
    TestVersions('1', '2');
    TestVersions('2', '1');
    TestVersions('3', '3');

    TestVersions('1.1', '1');
    TestVersions('2.1', '1');
    TestVersions('1.1', '2');

    TestVersions('2.12.11', '2.12.25');
    TestVersions('', '2.12.25');
    TestVersions('2.12.25', '');
    TestVersions('', '');
    TestVersions('2.12.11', '2.012.11');
{ }
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
end;

function InitializeUninstall(): Boolean;
begin
  // check if client is running
  if IsModuleLoadedU( 'BlinkStickClient.exe' ) then
  begin
    MsgBox( 'BlinkStick Client is running, please close it and run again uninstall.', mbError, MB_OK );
    Result := false;
  end
  else
  begin 
    Result := true;
  end;

  // Unload the DLL, otherwise the dll psvince is not deleted
  UnloadDLL(ExpandConstant('{app}\psvince.dll'));

end;

function NeedRestart(): Boolean;
begin
  if (delayedReboot Or RequireRestart) then
		Result := true;
end;


// Code below taken from 
// http://stackoverflow.com/questions/2000296/innosetup-how-to-automatically-uninstall-previous-installed-version

function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

//End of code snippet from stackoverflow.com
