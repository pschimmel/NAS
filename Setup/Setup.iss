#define ApplicationName 'Network Analysis System'
#define ApplicationShortName 'NAS'
#define ApplicationVersion '4.0.0'
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
AppCopyright=© 2015-2024 by {#ApplicationCompany}
DefaultDirName={autopf}\{#ApplicationCompanyShort}\{#ApplicationName}
DefaultGroupName={#ApplicationCompanyShort}\{#ApplicationName}
SolidCompression=true
Compression=lzma2/Ultra64
InternalCompressLevel=ultra64
UsePreviousAppDir=true
OutputBaseFilename=Setup_{#ApplicationShortName}_{#ApplicationVersion}
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
Name: "Application"; Description: "Application"; Types: full compact custom; Flags: fixed
Name: "Reports"; Description: "Reports"; Types: full compact custom; Flags: fixed
Name: "Examples"; Description: "Examples"; Types: full custom

[Dirs]
Name: "{app}"; Components: Application Reports Examples; Permissions: everyone-full

[Files]
Source: "..\src\Output\bin\release\net8.0-windows\*.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\src\Output\bin\release\net8.0-windows\*.exe"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "..\src\Output\bin\release\net8.0-windows\*.json"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Application
Source: "Examples\*.nas"; DestDir: "{app}\Examples"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Examples
Source: "Examples\*.XER"; DestDir: "{app}\Examples"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; Components: Examples

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