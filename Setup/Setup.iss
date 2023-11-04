#define ApplicationName 'Network Analysis System'
#define ApplicationShortName 'NAS'
#define ApplicationVersion '3.0.0'
#define ApplicationCompany 'Engineering Solutions'
#define ApplicationCompanyShort 'Engineering Solutions'

[Setup]
AppName={#ApplicationName}
AppVersion={#ApplicationVersion}
AppVerName={cm:NameAndVersion,{#ApplicationShortName},{#ApplicationVersion}}
VersionInfoVersion={#ApplicationVersion}
VersionInfoProductVersion={#ApplicationVersion}
VersionInfoProductTextVersion={#ApplicationName} {#ApplicationVersion}
VersionInfoCompany={#ApplicationCompany}
VersionInfoCopyright=Copyright 2014-2021 by {#ApplicationCompany}
AppPublisher={#ApplicationCompany}
AppPublisherURL=http://engineeringsolutions.de
AppSupportURL=http://engineeringsolutions.de
AppUpdatesURL=http://engineeringsolutions.de
UsedUserAreasWarning=no
AppCopyright=© 2015-2021 by {#ApplicationCompany}
DefaultDirName={autopf}\{#ApplicationCompanyShort}\{#ApplicationName}
DefaultGroupName={#ApplicationCompanyShort}\{#ApplicationName}
SolidCompression=true
Compression=lzma2/Ultra64
InternalCompressLevel=ultra64
UsePreviousAppDir=true
OutputDir=.
OutputBaseFilename=Setup_NASPro
WizardImageFile=NormalImage.bmp
WizardSmallImageFile=SmallImage.bmp
ShowLanguageDialog=auto
ChangesAssociations=true
WizardImageStretch=false
AppId={{20DD815A-97BC-4BFD-A4C0-773E414DE257}
SetupIconFile=Setup.ico
UninstallLogMode=overwrite
UninstallDisplayIcon={app}\NAS.exe
DisableWelcomePage=False
WizardStyle=Modern
ShowTasksTreeLines=True

[Components]
Name: "Application"; Description: "Anwendung"; Types: full compact custom; Flags: fixed
Name: "Reports"; Description: "Berichte"; Types: full compact custom; Flags: fixed
Name: "Examples"; Description: "Beispiele"; Types: full custom
Name: "Docs"; Description: "Dokumentation"; Types: full custom
Name: "dotnet"; Description: ".NET Framework"; Types: full custom

[Dirs]
Name: "{app}"; Components: Application Docs Reports Examples ; Permissions: everyone-full

[Files]
Source: "..\Output\bin\Release\NAS.Base.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.Config.exe"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.Controllers.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.DB.dll"; DestDir: "{app}"; Flags: ignoreversion sortfilesbyextension replacesameversion; Components: Application; Permissions: everyone-full
Source: "..\Output\bin\Release\NAS.DB.MySQL.dll"; DestDir: "{app}"; Flags: ignoreversion sortfilesbyextension replacesameversion; Components: Application; Permissions: everyone-full
Source: "..\Output\bin\Release\NAS.DB.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion sortfilesbyextension replacesameversion; Components: Application; Permissions: everyone-full
Source: "..\Output\bin\Release\NAS.DB.SQLServerCE.dll"; DestDir: "{app}"; Flags: ignoreversion sortfilesbyextension replacesameversion; Components: Application; Permissions: everyone-full
Source: "..\Output\bin\Release\NAS.exe"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.exe.config"; DestDir: "{app}"; Flags: ignoreversion sortfilesbyextension replacesameversion; Components: Application; Permissions: everyone-full
Source: "..\Output\bin\Release\NAS.Globalization.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.ImportExport.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.Model.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.Scheduling.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.View.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NAS.ViewModel.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\ES.Tools.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\ES.WPF.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\ES.WPF.Toolkit.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application

Source: "..\Output\bin\Release\AvalonDock.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\AvalonDock.Themes.*.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\BouncyCastle.Crypto.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\ControlzEx.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\DotNetProjects.Wpf.Extended.Toolkit.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\EntityFramework.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\EntityFramework.SqlServer.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\EntityFramework.SqlServerCompact.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\FastReport.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\FastReport.Bars.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\FastReport.Editor.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Fluent.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\GongSolutions.WPF.DragDrop.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Google.Protobuf.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\K4os.Compression.LZ4.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\K4os.Compression.LZ4.Streams.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\K4os.Hash.xxHash.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\MySql.Data.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\MySql.Data.Entity.EF6.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\NHotkey*.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Renci.SshNet.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\RestSharp.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\System.Data.SQLite*.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\System.Data.SqlServerCe.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\System.Memory.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Ubiety.Dns.Core.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\amd64\*.*"; DestDir: "{app}\amd64"; Flags: IgnoreVersion sortfilesbyextension replacesameversion recursesubdirs; Components: Application
Source: "..\Output\bin\Release\x86\*.*"; DestDir: "{app}\amd64"; Flags: IgnoreVersion sortfilesbyextension replacesameversion recursesubdirs; Components: Application
Source: "..\Output\bin\Release\de\*.dll"; DestDir: "{app}\de"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\es\*.dll"; DestDir: "{app}\es"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\fr\*.dll"; DestDir: "{app}\fr"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\hu\*.dll"; DestDir: "{app}\hu"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\it\*.dll"; DestDir: "{app}\it"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\pt-BR\*.dll"; DestDir: "{app}\pt-BR"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\ro\*.dll"; DestDir: "{app}\ro"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\ru\*.dll"; DestDir: "{app}\ru"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\sv\*.dll"; DestDir: "{app}\sv"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\zh-Hans\*.dll"; DestDir: "{app}\zh-Hans"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Localization\*.frl"; DestDir: "{app}\Localization"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\Output\bin\Release\Reports\*.*"; DestDir: "{app}\Reports"; Flags: IgnoreVersion sortfilesbyextension replacesameversion recursesubdirs; Components: Reports
Source: "Examples\*.nas"; DestDir: "{app}\Examples"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Examples
Source: "Examples\*.XER"; DestDir: "{app}\Examples"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Examples
Source: "ndp48-web.exe"; DestDir: "{tmp}"; Flags: IgnoreVersion deleteafterinstall replacesameversion; Components: dotnet
Source: "FastReport.NET license.rtf"; DestDir: "{app}\Docs"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Docs
Source: "Erste Schritte.pdf"; DestDir: "{app}\Docs"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Docs
Source: "FRNetUserManual.chm"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; 

[Icons]
Name: "{group}\{#ApplicationName}"; Filename: "{app}\NAS.exe"; WorkingDir: "{app}"; IconFilename: "{app}\NAS.exe"; Comment: "Analysiert Netzpläne"
Name: "{group}\{#ApplicationShortName} Config"; Filename: "{app}\NAS.Config.exe"; WorkingDir: "{app}"; IconFilename: "{app}\NAS.Config.exe"; Comment: "Ändern der Datenbankeinstellungen"
Name: "{group}\{cm:ProgramOnTheWeb,NASPro}"; Filename: "http://www.gs-cms.com/"
Name: "{group}\{cm:UninstallProgram,NASPro}"; Filename: "{uninstallexe}"
Name: "{userdesktop}\{#ApplicationName}"; Filename: "{app}\NAS.exe"; WorkingDir: "{app}"; IconFilename: "{app}\NAS.exe"; Comment: "Analysiert Netzpläne"; Tasks: desktopicon

[Run]
Filename: "{app}\NAS.exe"; Flags: nowait postinstall skipifdoesntexist; Description: "{cm:LaunchProgram,{#ApplicationName}}"
Filename: "{app}\Docs\Erste Schritte.pdf"; Flags: nowait postinstall unchecked skipifdoesntexist skipifsilent shellexec; Description: "Dokument ""Erste Schritte"" öffnen"
Filename: "{app}\NAS.Config.exe"; Flags: nowait postinstall unchecked skipifdoesntexist skipifsilent; Description: "{cm:LaunchProgram,{#ApplicationShortName} Config}"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: Lizenz.rtf; InfoBeforeFile: Setup.rtf; 
Name: "french"; MessagesFile: "compiler:Languages\French.isl"; LicenseFile: Lizenz.rtf; InfoBeforeFile: Setup.rtf; 
Name: "german"; MessagesFile: "compiler:Languages\German.isl"; LicenseFile: Lizenz.rtf; InfoBeforeFile: Setup.rtf; 
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"; LicenseFile: Lizenz.rtf; InfoBeforeFile: Setup.rtf; 
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"; LicenseFile: Lizenz.rtf; InfoBeforeFile: Setup.rtf; 

[Registry]
Root: HKCR; SubKey: ".nas"; ValueType: string; ValueData: "Network Analysis System"; Flags: uninsdeletekey
Root: HKCR; SubKey: "Network Analysis System"; ValueType: string; ValueData: "Network Analysis System Schedule"; Flags: uninsdeletekey
Root: HKCR; SubKey: "Network Analysis System\Shell\Open\Command"; ValueType: string; ValueData: """{app}\NAS.exe"" ""%1"""; Flags: uninsdeletekey
Root: HKCR; Subkey: "Network Analysis System\DefaultIcon"; ValueType: string; ValueData: "{app}\NAS.exe,0"; Flags: uninsdeletevalue

[PreCompile]
;Name: "..\..\Confuser\Confuser.Console.exe"; Parameters: '..\NASPro 2\Setup\NAS_Confused.crproj'; Flags: runminimized abortonerror cmdprompt redirectoutput
;Name: "..\..\ConfuserEx\Confuser.CLI.exe"; Parameters: "'..\NASPro 2\Setup\NAS_ConfusedEx.crproj' -out='..\NASPro 2\Setup\Confused' -n"; Flags: runminimized abortonerror cmdprompt redirectoutput

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.//
// See https://kynosarges.org/DotNetVersion.html
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//    'v4.7'          .NET Framework 4.7
//    'v4.7.1'        .NET Framework 4.7.1
//    'v4.7.2'        .NET Framework 4.7.2
//    'v4.8'          .NET Framework 4.8
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 before Win10 November Update
          'v4.6.2': versionRelease := 394802; // 394806 before Win10 Anniversary Update
          'v4.7':   versionRelease := 460798; // 460805 before Win10 Creators Update
          'v4.7.1': versionRelease := 461308; // 461310 before Win10 Fall Creators Update
          'v4.7.2': versionRelease := 461808; // 461814 before Win10 April 2018 Update
          'v4.8':   versionRelease := 528040; // 528049 before Win10 May 2019 Update
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

var
  UsagePage: TInputOptionWizardPage;
  
procedure InitializeWizard;
begin
  { Create the pages }  
  UsagePage := CreateInputOptionPage(wpInfoBefore,
    'Installation Type', 'How will you use My Program?',
    'Please specify how you would like to use My Program, then click Next.',
    True, False);
  UsagePage.Add('Standalone Installation (Local SQLite Database)');
  UsagePage.Add('Server Installation');

  { Set default values, using settings that were stored last time if possible }
  case GetPreviousData('UsageMode', '') of
    'standalone': UsagePage.SelectedValueIndex := 0;
    'server': UsagePage.SelectedValueIndex := 1;
  else
    UsagePage.SelectedValueIndex := 0;
  end;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
var
  UsageMode: String;
begin
  { Store the settings so we can restore them next time }
  case UsagePage.SelectedValueIndex of
    0: UsageMode := 'standalone';
    1: UsageMode := 'server';
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo,
  MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S: String;
begin
  { Fill the 'Ready Memo' with the normal settings and the custom settings }
  S := '';
  S := S + 'Usage Mode:' + NewLine + Space;
  case UsagePage.SelectedValueIndex of
    0: S := S + 'Standalone Installation';
    1: S := S + 'Server Installation';
  end;
  S := S + NewLine + NewLine;        
  S := S + MemoDirInfo + NewLine;      
  Result := S;
end;

function CreateBatch(): boolean;
var
  fileName : string;
  dbPath : string;
  lines : TArrayOfString;
begin
  Result := true;
  fileName := ExpandConstant('{localappdata}\NAS\NAS.Config.xml');
  dbPath := ExpandConstant('{localappdata}\NAS\NAS.sqlite3'); 
  SetArrayLength(lines, 4);
  lines[0] := '<Config>';
  lines[1] := '  <DatabaseType>SQLite</DatabaseType>';
  lines[2] := '  <FileName>' + dbPath + '</FileName>';
  lines[3] := '</Config>';
  Log('File Name: ' + fileName);
  Log('DB Type:    SQLite');
  Log('DB Path:   ' + dbPath);
  Result := SaveStringsToFile(filename, lines, true);  
  exit;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ErrorCode: Integer;
begin
  if (CurStep=ssPostInstall) then begin
    if (UsagePage.SelectedValueIndex = 0) then begin
      CreateBatch();
    end;
    
    if not (IsDotNetDetected('v4.8', 0)) and WizardIsComponentSelected('dotnet') then begin
      if MsgBox('Das erforderliche .NET-Framework 4.8 ist auf Ihrem Rechner nicht installiert. Möchten Sie es jetzt installieren (Es wird eine Internetverbindung benötigt)?', mbCriticalError, MB_YESNO) = IDNO then begin
        Abort;
      end;
      if (not ShellExec('open', ExpandConstant('{tmp}\ndp48.exe'), '/norestart /passive /1031', '', SW_ShowNormal, ewWaitUntilTerminated, ErrorCode)) or (ErrorCode <> 0) then begin
        MsgBox('Fehler bei der Installation des .NET Frameworks. Das Setup wird abgebrochen', mbCriticalError, MB_OK);
        Abort;
      end;
    end;
  end;
end;
