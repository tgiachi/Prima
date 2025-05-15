# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="0.15.0"></a>
## [0.15.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.15.0) (2025-05-15)

### Features

* **Deflate.cs:** add Deflate class for compression using LibDeflateBinding ([db5f721](https://www.github.com/tgiachi/Prima/commit/db5f72152a654d77deba9528a099a11edd362726))
* **MultiData.cs:** add new MultiData class to handle loading and configuring multi data for the application ([3c40054](https://www.github.com/tgiachi/Prima/commit/3c400541abbbb39395a14a464efae847695495e7))

<a name="0.14.0"></a>
## [0.14.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.14.0) (2025-05-15)

### Features

* added dictionary from UOX3 (thanks guys) ([6a09ae7](https://www.github.com/tgiachi/Prima/commit/6a09ae76306ae6055a47f5841b081029e8b9c8a4))
* **CommandsScriptModule.cs:** add logging functionality to log executed commands for debugging purposes ([68b66bf](https://www.github.com/tgiachi/Prima/commit/68b66bf7c46f3f03f7cafbbcb1cb2ab84e57fb53))
* **ConsoleCommandService.cs:** add System.Diagnostics namespace for using Process class ([a0d4bc7](https://www.github.com/tgiachi/Prima/commit/a0d4bc70fabf6312a6d6c0c19b7dc897ee403e3c))
* **Point3D.cs, BaseWorldEntity.cs, FodyWeavers.xml, Prima.UOData.csproj:** implement INotifyPropertyChanged interface to enable property change notifications in Point3D and BaseWorldEntity classes ([3864595](https://www.github.com/tgiachi/Prima/commit/3864595c46bffd44f8a35f25d6a049586d194ff7))
* **Prima.sln:** add Prima.JavaScript.Engine project to the solution ([9a05d13](https://www.github.com/tgiachi/Prima/commit/9a05d1334f19fe4649c6b86509e22e1d9615fb5a))
* **Program.cs:** add Prima.UOData.Modules.Scripts namespace for new script modules ([1567ef2](https://www.github.com/tgiachi/Prima/commit/1567ef2c99bbcbe897f6fb30e3ac559ac877a15a))
* **ShardConfig.cs:** update TimeZone property to dynamically set based on local time ([3dfff2f](https://www.github.com/tgiachi/Prima/commit/3dfff2f7fd62ad5a27b0ae7e1835740d19a6acc4))

<a name="0.13.0"></a>
## [0.13.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.13.0) (2025-05-14)

### Features

* **NetworkSession.cs:** add SetProperty and GetProperty methods to manage session properties ([22681fe](https://www.github.com/tgiachi/Prima/commit/22681feb7213cd2e610ecf116d05b6d0355bb18f))

<a name="0.12.0"></a>
## [0.12.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.12.0) (2025-05-14)

### Features

* add AutosaveConfig class to handle autosave interval in minutes ([76b012b](https://www.github.com/tgiachi/Prima/commit/76b012bd1e2b92f14977b0625fc38c157b3cc22f))
* **BinaryWriterExtension.cs:** add BinaryWriterExtension class with methods to write strings, Point2D, Point3D, and Serial types to a binary writer ([944a856](https://www.github.com/tgiachi/Prima/commit/944a856b7b738adbb21e9c34efd6a332e93f3798))
* **PersistenceManager.cs:** add Version constant for serialization, change magicNumber to byte array, add entitySerializersAsType dictionary for better entity type lookup ([9439e89](https://www.github.com/tgiachi/Prima/commit/9439e893a7249b089fac19adcbcc0a9800b9be55))
* **Prima.Core.Server:** add SerializableHeaderAttribute to define custom serialization headers for entities ([134292e](https://www.github.com/tgiachi/Prima/commit/134292e8fb1e14884be43ba9408ced143aa8ba76))
* **Serialization:** introduce new file SerializationDataFile and SerializationEntryData to improve data serialization structure ([db87d30](https://www.github.com/tgiachi/Prima/commit/db87d306c8bb23985b75f406a9a18b90523db60f))
* **World:** add WorldSavedEvent and WorldSavingEvent classes to handle world saving events ([1a48778](https://www.github.com/tgiachi/Prima/commit/1a48778e101285dc99120384976249fbda7898fb))
* **WorldManagerService:** add SaveWorldAsync method to IWorldManagerService interface for saving world data asynchronously ([62cbf5f](https://www.github.com/tgiachi/Prima/commit/62cbf5f448ec2bce0acc07ba8a6245e04191b66c))

### Bug Fixes

* **SerializableHeaderAttribute.cs:** remove fileName parameter from SerializableHeaderAttribute constructor as it is not used ([dd3d95a](https://www.github.com/tgiachi/Prima/commit/dd3d95a8e3dca752225205c2be410fc190f2972e))

<a name="0.11.1"></a>
## [0.11.1](https://www.github.com/tgiachi/Prima/releases/tag/v0.11.1) (2025-05-12)

<a name="0.11.0"></a>
## [0.11.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.11.0) (2025-05-12)

### Features

* **CharacterCreationHandler.cs:** add IMapService dependency to CharacterCreationHandler to handle starting location mapping ([b04ef3c](https://www.github.com/tgiachi/Prima/commit/b04ef3c021fdf06c768b977101960cec07a11ce6))
* **CharacterCreationHandler.cs:** add support for character creation event handling and ([cc757d5](https://www.github.com/tgiachi/Prima/commit/cc757d5c083e56b58d87fa4fa19732b330db7787))
* **ClientConfigurationService.cs:** add RaceDefinitions.Configure() method call to initialize race definitions in the ClientConfigurationService class ([53bc001](https://www.github.com/tgiachi/Prima/commit/53bc0015e20368b62250efeb329a38bfe69f6abc))
* **FileScriptModule.cs:** add IncludeScripts method to FileScriptModule for executing all script files in a specified directory ([9f5dcb0](https://www.github.com/tgiachi/Prima/commit/9f5dcb0bd73c83eff330cd67683990b9c86d6db8))
* **Prima.Core.Server.csproj:** update Orion.Core.Server package version to 0.28.3 ([a07312e](https://www.github.com/tgiachi/Prima/commit/a07312eb3298d879d1579a8015f05c2c56531526))
* **TimerService.cs:** implement TimerService to manage timers with registration, unregistration, and callback execution ([ed63566](https://www.github.com/tgiachi/Prima/commit/ed635667dcf81f9e39bdfbeae342e170636f71e7))
* **UOData:** add support for delay in timer registration to allow for delayed timer execution ([6614d4b](https://www.github.com/tgiachi/Prima/commit/6614d4be0f7ee1452d0a7ae0da7b8e82a8cbe32d))

<a name="0.10.1"></a>
## [0.10.1](https://www.github.com/tgiachi/Prima/releases/tag/v0.10.1) (2025-05-12)

### Bug Fixes

* **ConnectToGameServer.cs:** change how IP address bytes are read to correctly assign values ([c25337f](https://www.github.com/tgiachi/Prima/commit/c25337f65c92dff8c20adf474671303a00919f10))

<a name="0.10.0"></a>
## [0.10.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.10.0) (2025-05-12)

### Features

* **Prima.Core.Server.csproj:** update Orion.Core.Server package version to 0.28.0 ([e011796](https://www.github.com/tgiachi/Prima/commit/e0117967602c0af5c9ee65df4df31e21bd04b301))
* **Prima.Network:** add IpAddressExtensions class to handle IP address conversions ([13ab025](https://www.github.com/tgiachi/Prima/commit/13ab0259e1b7a1f9ec939088ee1604bb58c06ae0))
* **Program.cs:** add ISerialGeneratorService to service collection for generating serials ([7603ef2](https://www.github.com/tgiachi/Prima/commit/7603ef2a346fc8e1b4717efa1ec5efbd17b7b524))
* **SerialProgressionEntity.cs:** create a new entity class SerialProgressionEntity ([4f64012](https://www.github.com/tgiachi/Prima/commit/4f64012a875a85781ba9db6284e9e1846bba1dad))
* **UserLoginContext.cs:** add UserLoginContext record for user login information ([c800225](https://www.github.com/tgiachi/Prima/commit/c8002257da09f2c7be42a3c5cca03ec88a6f3791))

<a name="0.9.0"></a>
## [0.9.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.9.0) (2025-05-09)

### Features

* **CharacterCreation.cs:** add new properties for character creation including skills, profession, and attributes ([c7c0afb](https://www.github.com/tgiachi/Prima/commit/c7c0afbd8c0877603a19d61d731d6515a61ed583))
* **skills.json:** add a new skills.json file with detailed information about various skills including SkillID, Name, Title, scaling factors for Strength, Dexterity, and Intelligence, StatTotal, Gain factors, ProfessionSkillName, PrimaryStat, and SecondaryStat for each skill. ([7b67b52](https://www.github.com/tgiachi/Prima/commit/7b67b52341fe222894d4aec464fd3d9ee938239e))
* **UOData:** add support for processing profession configuration files to build ([5fbfe04](https://www.github.com/tgiachi/Prima/commit/5fbfe041a6624aa02906297e9143d2885e8730a8))

<a name="0.8.0"></a>
## [0.8.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.8.0) (2025-05-09)

### Features

* **Prima.Network.csproj:** update package references to remove unnecessary whitespace ([9a859a0](https://www.github.com/tgiachi/Prima/commit/9a859a028945db7b8052524beafa14d9ea5820fd))
* **UOData:** add support for character list packet with starting locations and characters ([3e86c23](https://www.github.com/tgiachi/Prima/commit/3e86c23caa71151838459464b8f5a2dfb7899002))

<a name="0.7.0"></a>
## [0.7.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.7.0) (2025-05-08)

### Features

* **csproj:** update Orion.Core.Server, Orion.Network.Core, and Orion.Network.Tcp ([9ea7555](https://www.github.com/tgiachi/Prima/commit/9ea7555712d0084780860ff2465912983dea7778))
* **LoginHandler:** update FeatureFlagsResponse to use ExpansionInfo from UOContext for supported features ([fbb956b](https://www.github.com/tgiachi/Prima/commit/fbb956bcda47403e792b807dd59299ff56afeb06))
* **UOData:** add support for loading expansion configuration and setting expansion info ([9c37554](https://www.github.com/tgiachi/Prima/commit/9c37554bc232fa4e95677c73fbd1e686b3744bd1))

<a name="0.6.1"></a>
## [0.6.1](https://www.github.com/tgiachi/Prima/releases/tag/v0.6.1) (2025-05-08)

<a name="0.6.0"></a>
## [0.6.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.6.0) (2025-05-08)

### Features

* **ClientVersion.cs:** remove outdated ClientVersion class and its dependencies ([03372d5](https://www.github.com/tgiachi/Prima/commit/03372d5105fa0b8874820cf63dc0f393cfea3ad2))
* **docs:** add docfx.json configuration file for building documentation with metadata and build settings ([84837bd](https://www.github.com/tgiachi/Prima/commit/84837bd2d655994fd3b5e660a00a88337f80d623))
* **Prima.Network.csproj:** update Orion packages to version 0.25.0 for compatibility and new features ([9bef184](https://www.github.com/tgiachi/Prima/commit/9bef1846f2c1b9505d82032edb295e15da010b47))
* **ShardConfig.cs:** add ClientVersion property to ShardConfig for storing client version information ([6ac07b9](https://www.github.com/tgiachi/Prima/commit/6ac07b97978e50efae6c80392d7f5def62e3ce57))

<a name="0.5.0"></a>
## [0.5.0](https://www.github.com/tgiachi/Prima/releases/tag/v0.5.0) (2025-05-07)

### Features

* **AccountResult.cs:** add AccountResult class to handle success and failure results for account operations ([cb2783c](https://www.github.com/tgiachi/Prima/commit/cb2783c7b1d2342398ea56a03efea344378d514b))
* **Assets:** add Prima logo image and MIT License file to project ([6f3c081](https://www.github.com/tgiachi/Prima/commit/6f3c081251fb73a99c84b5c6921f540284689e55))
* **ChangePasswordObject.cs:** add ChangePasswordObject class to handle password change data ([6c93c49](https://www.github.com/tgiachi/Prima/commit/6c93c496f615af1ec6f7ded06454e7178e71d206))
* **ClientVersion.cs:** add ClientVersion class to manage different client versions and their properties ([1c2d440](https://www.github.com/tgiachi/Prima/commit/1c2d44029460f51932f8f318c976a98afbc6c11e))
* **CommandDefinitionData.cs:** add Execute property to CommandDefinitionData class for defining command execution logic ([61333ee](https://www.github.com/tgiachi/Prima/commit/61333ee3f0627bedaee8b7c60c3a4c4021b70d2f))
* **commands:** add new classes for command definition, result, parsed data ([140a9cf](https://www.github.com/tgiachi/Prima/commit/140a9cfe3468445bb9500be71139c4c3f4b9d3ea))
* **CommandSystemService.cs:** add new 'clear' and 'help' commands with respective logic and descriptions ([a7c9f7d](https://www.github.com/tgiachi/Prima/commit/a7c9f7d6d059d777d20d9ce8aa0d975ef412cd43))
* **ConnectionHandler.cs:** update ConnectionHandler to use ClientVersionRequest instead of ClientVersion for better clarity and consistency ([496ea82](https://www.github.com/tgiachi/Prima/commit/496ea8204efeaabff93861167bb593dcd9e4ed08))
* **ConnectToGameServer.cs:** add new packet class to handle connecting to game server with necessary properties and methods ([1283a70](https://www.github.com/tgiachi/Prima/commit/1283a708c2cab4fad00079f1b9c1d00e98e43538))
* **csproj:** update Orion.Core.Server, Orion.Network.Core, and Orion.Network.Tcp package versions to 0.20.0 for compatibility and new features ([c890d01](https://www.github.com/tgiachi/Prima/commit/c890d01830a9169113d4f1c540d9f3f8a16eed00))
* **data:** add expansions.json and map-definitions.json files with expansion and map data ([cc7c8b5](https://www.github.com/tgiachi/Prima/commit/cc7c8b52d542265db480c0f811c77ceea45d9372))
* **dependabot.yml:** add Dependabot configuration for NuGet packages and GitHub Actions to automate updates and enhance repository maintenance ([c7cb913](https://www.github.com/tgiachi/Prima/commit/c7cb913c2403412573a9b7a48c6c01e038d671fd))
* **Dockerfile:** add Dockerfile to set up multi-stage build for the application ([a4d284c](https://www.github.com/tgiachi/Prima/commit/a4d284c18a16fcd945670715b3ff3e4901b69f58))
* **Dockerfile:** add HEALTHCHECK instruction to Dockerfile for monitoring app health ([f120a96](https://www.github.com/tgiachi/Prima/commit/f120a9637f13a283863985928d96fbc81b4e8ebc))
* **email:** add IEmailService interface to define email service contracts ([bccc842](https://www.github.com/tgiachi/Prima/commit/bccc8428fd8e56fde5a1d14f020d59c559594c36))
* **Entry3D.cs, Entry5D.cs:** add new data structures Entry3D and Entry5D to store specific information with proper struct layout ([ef2e7bb](https://www.github.com/tgiachi/Prima/commit/ef2e7bb6c055d8365108e54815544ad1865c6219))
* **EventLoopService.cs:** implement event loop service for executing actions with different priorities ([0c23397](https://www.github.com/tgiachi/Prima/commit/0c2339771343faf9da84220d747f1de098f07526))
* **FeatureFlags.cs:** add new class FeatureFlags to handle feature flags sent from server to client ([e9c8881](https://www.github.com/tgiachi/Prima/commit/e9c8881d54fd12958e975fb1fc6e1428b1dec14d))
* **FlagsConverter.cs:** add FlagsConverter class to handle conversion of flags in JSON format ([4564d9d](https://www.github.com/tgiachi/Prima/commit/4564d9d1b0b078db01838009ded562b8029e6859))
* **IAccountManager.cs:** add admin and isVerified parameters to CreateAccountAsync method for more flexibility in account creation ([494f93f](https://www.github.com/tgiachi/Prima/commit/494f93f8b18263140cd1523a29b445cb204bd262))
* **IAccountManager.cs:** extend IAccountManager interface to include IOrionStartService for additional functionality ([b895d1f](https://www.github.com/tgiachi/Prima/commit/b895d1fd5beece6015fdaf20e1aaa60451cccf8e))
* **INetworkService.cs:** extend INetworkService interface with IDisposable for proper resource management ([56c86ba](https://www.github.com/tgiachi/Prima/commit/56c86ba35df204f8291a42719186be46ccbbe83d))
* **MulFileReaderService.cs:** add MulFileReaderService class to handle reading Mul files and start/stop tasks ([cd5261c](https://www.github.com/tgiachi/Prima/commit/cd5261c57585022e8325a76d28d3c0f5e5a9207c))
* **NetworkService.cs:** add INetworkPacketListener interface to handle network packet events ([4c279e7](https://www.github.com/tgiachi/Prima/commit/4c279e7ec3831965734181d2d6ed7ee08bdb0c4b))
* **NetworkService.cs:** refactor NetworkService to handle multiple packets in a single message ([83ea3bf](https://www.github.com/tgiachi/Prima/commit/83ea3bfdcac0a47dc0aeed73039d8844ce97439e))
* **NetworkSession.cs:** add AuthId property to NetworkSession class for authentication ([647f8f3](https://www.github.com/tgiachi/Prima/commit/647f8f3fdf9c0d88e70de0bf328dcffdb73fde44))
* **NetworkSession.cs:** add DisconnectDelegate event to handle session disconnections ([66010f1](https://www.github.com/tgiachi/Prima/commit/66010f1f7da0507c7a822b8945beb0d24ff2db50))
* **NetworkSession.cs:** add event handler for sending packets and properties for Seed and ClientVersion ([9665fc3](https://www.github.com/tgiachi/Prima/commit/9665fc3e60fffb0696f60d1eef8340dc4e8dc0e0))
* **NetworkSession.cs:** add NetworkSession class to handle network sessions ([079e338](https://www.github.com/tgiachi/Prima/commit/079e338b80bfb24cbcb16f5e1b4f07118e70708c))
* **Point2D.cs:** add a new Point2D struct to represent a 2D point with X and Y coordinates ([3cb88d8](https://www.github.com/tgiachi/Prima/commit/3cb88d8b0c294137de720af108cc792a43609bc0))
* **Prima.Core.Server:** add PrimaServerConfig and PrimaServerOptions classes ([f89b686](https://www.github.com/tgiachi/Prima/commit/f89b6867c91e0de5eb16d390e20655a763627b27))
* **Prima.Core.Server:** add PrimaServerContext class to manage service provider and services ([033cfd0](https://www.github.com/tgiachi/Prima/commit/033cfd0d8e17b8dad9022b1c8c7282e39ecb2b00))
* **Prima.Core.Server.csproj:** update Orion.Core.Server package version to 0.15.0 ([2177140](https://www.github.com/tgiachi/Prima/commit/21771401290e06383b325f64200c512e7ebf6f6a))
* **Prima.Core.Server.csproj:** update Orion.Core.Server package version to 0.22.3 ([43522b5](https://www.github.com/tgiachi/Prima/commit/43522b54b0dcfc116c692ff2df0b65601e9e9f8c))
* **Prima.Network:** add IPacketManager interface for managing network packets ([088fbe8](https://www.github.com/tgiachi/Prima/commit/088fbe8bb58a49997b3e44389278c43b65ae54c6))
* **Prima.Network.csproj:** update Orion packages to version 0.24.0 for compatibility and new features ([0b62ced](https://www.github.com/tgiachi/Prima/commit/0b62ced83bdd665b81cf3ff274457a2b07fb08d1))
* **Prima.sln:** add new project Prima.UOData to the solution ([15102b4](https://www.github.com/tgiachi/Prima/commit/15102b40460669235cc2c239b205c0d2fc9c42f7))
* **Prima.sln:** add Prima.Tcp.Test project to the solution file ([8b3d7f4](https://www.github.com/tgiachi/Prima/commit/8b3d7f46bdc6a2a3d8d03ccaef584a8600c48e90))
* **Prima.sln:** rename project "Prima.Network.Packets" to "Prima.Network" ([964075e](https://www.github.com/tgiachi/Prima/commit/964075e73de086293789c6e1ec7652eaa5053cbc))
* **Prima.UOData:** add IMulFileReaderService interface for Mul file reading service ([6392d40](https://www.github.com/tgiachi/Prima/commit/6392d40472e5ddff10bc98cd11c4802cdecde68d))
* **Prima.UOData:** add interfaces IPoint2D and IPoint3D to define 2D and 3D points in geometry for future use in the project ([f0c4cc6](https://www.github.com/tgiachi/Prima/commit/f0c4cc6fd1e274389059bc7f63cb9dc32695628f))
* **Prima.UOData:** add new classes and structures for handling UO data files ([1b63efa](https://www.github.com/tgiachi/Prima/commit/1b63efaae4abd125938ae9859dae5ae0a5cbfa6b))
* **Prima.UOData:** add new enum types for ClientFlags, ClientType, ProtocolChanges, and UoFileType to define various client flags, types, protocol changes, and file types used in the application. ([88bd077](https://www.github.com/tgiachi/Prima/commit/88bd0777b1e63275e3b649350a66bd0b6e23ae89))
* **PrimaServerConfig.cs:** add TcpServerConfig class to handle TCP server configuration ([01fc825](https://www.github.com/tgiachi/Prima/commit/01fc825109ef27df1299d833bf0ff2b9370c5fc1))
* **Program.cs:** add MetricsRoute to map metrics-related routes in the application ([b2b1e80](https://www.github.com/tgiachi/Prima/commit/b2b1e809cbafa81f606b08438aee69383cb351cb))
* **scripts:** add EventScriptModule, SchedulerModule, and VariableModule to handle ([13a630c](https://www.github.com/tgiachi/Prima/commit/13a630ca7fc7d8d640e069c73aa8e43e0f6032f3))
* **Serial.cs:** add new Serial struct to handle unique identifiers in the application ([163ad4b](https://www.github.com/tgiachi/Prima/commit/163ad4b089f4588437257b830959b6f9f469c5c8))
* **server:** add JwtAuthConfig, ShardConfig, AccountServerConfig classes ([e50b578](https://www.github.com/tgiachi/Prima/commit/e50b57816fc9cf87248d45c16e22cde8b3f86f26))
* **sln:** add Prima.Core.Server project to the solution for modularization ([d48e80f](https://www.github.com/tgiachi/Prima/commit/d48e80f7ff6a6ece98a70b6891a92613780f92f1))
* **TcpServerConfig.cs:** add default values for ServerName, LoginPort, and GamePort properties ([6590466](https://www.github.com/tgiachi/Prima/commit/6590466c2b08bab8173b0eb6e5402163208241eb))
* **TileData.cs:** add new file TileData.cs containing structs LandData and ItemData ([dfddc0a](https://www.github.com/tgiachi/Prima/commit/dfddc0ad19c141fe5d03a83cd217994838657ad7))

### Bug Fixes

* **BasePacketListenerHandler.cs:** update GetSession method in SessionService to include a parameter for checking session existence ([055ff55](https://www.github.com/tgiachi/Prima/commit/055ff558f889710216c5d435fb9fe52a22a2313f))
* **docker_image.yml:** update Docker image names from orionirc-server to prima-server to match project name ([cbba6fe](https://www.github.com/tgiachi/Prima/commit/cbba6fe4d38a1243b34f3cd8d0234245b9772b64))
* **EventLoopConfig.cs:** update TickIntervalMs value from 50 to 90 for slower event loop ticks ([621fc4a](https://www.github.com/tgiachi/Prima/commit/621fc4aea94bfff9904450578f7849e73fd858e0))
* **PacketsTests.cs:** update SessionKey value to a positive number to fix test failure due to negative value being used ([f9d8794](https://www.github.com/tgiachi/Prima/commit/f9d8794673468ecca228b80130dba59de24a457a))

<a name="0.0.1"></a>
## [0.0.1](https://www.github.com/tgiachi/Prima/releases/tag/v0.0.1) (2025-05-05)

### Features

* **AccountResult.cs:** add AccountResult class to handle success and failure results for account operations ([cb2783c](https://www.github.com/tgiachi/Prima/commit/cb2783c7b1d2342398ea56a03efea344378d514b))
* **Assets:** add Prima logo image and MIT License file to project ([6f3c081](https://www.github.com/tgiachi/Prima/commit/6f3c081251fb73a99c84b5c6921f540284689e55))
* **ChangePasswordObject.cs:** add ChangePasswordObject class to handle password change data ([6c93c49](https://www.github.com/tgiachi/Prima/commit/6c93c496f615af1ec6f7ded06454e7178e71d206))
* **CommandDefinitionData.cs:** add Execute property to CommandDefinitionData class for defining command execution logic ([61333ee](https://www.github.com/tgiachi/Prima/commit/61333ee3f0627bedaee8b7c60c3a4c4021b70d2f))
* **commands:** add new classes for command definition, result, parsed data ([140a9cf](https://www.github.com/tgiachi/Prima/commit/140a9cfe3468445bb9500be71139c4c3f4b9d3ea))
* **CommandSystemService.cs:** add new 'clear' and 'help' commands with respective logic and descriptions ([a7c9f7d](https://www.github.com/tgiachi/Prima/commit/a7c9f7d6d059d777d20d9ce8aa0d975ef412cd43))
* **ConnectToGameServer.cs:** add new packet class to handle connecting to game server with necessary properties and methods ([1283a70](https://www.github.com/tgiachi/Prima/commit/1283a708c2cab4fad00079f1b9c1d00e98e43538))
* **csproj:** update Orion.Core.Server, Orion.Network.Core, and Orion.Network.Tcp package versions to 0.20.0 for compatibility and new features ([c890d01](https://www.github.com/tgiachi/Prima/commit/c890d01830a9169113d4f1c540d9f3f8a16eed00))
* **dependabot.yml:** add Dependabot configuration for NuGet packages and GitHub Actions to automate updates and enhance repository maintenance ([c7cb913](https://www.github.com/tgiachi/Prima/commit/c7cb913c2403412573a9b7a48c6c01e038d671fd))
* **Dockerfile:** add Dockerfile to set up multi-stage build for the application ([a4d284c](https://www.github.com/tgiachi/Prima/commit/a4d284c18a16fcd945670715b3ff3e4901b69f58))
* **Dockerfile:** add HEALTHCHECK instruction to Dockerfile for monitoring app health ([f120a96](https://www.github.com/tgiachi/Prima/commit/f120a9637f13a283863985928d96fbc81b4e8ebc))
* **email:** add IEmailService interface to define email service contracts ([bccc842](https://www.github.com/tgiachi/Prima/commit/bccc8428fd8e56fde5a1d14f020d59c559594c36))
* **EventLoopService.cs:** implement event loop service for executing actions with different priorities ([0c23397](https://www.github.com/tgiachi/Prima/commit/0c2339771343faf9da84220d747f1de098f07526))
* **FeatureFlags.cs:** add new class FeatureFlags to handle feature flags sent from server to client ([e9c8881](https://www.github.com/tgiachi/Prima/commit/e9c8881d54fd12958e975fb1fc6e1428b1dec14d))
* **IAccountManager.cs:** add admin and isVerified parameters to CreateAccountAsync method for more flexibility in account creation ([494f93f](https://www.github.com/tgiachi/Prima/commit/494f93f8b18263140cd1523a29b445cb204bd262))
* **IAccountManager.cs:** extend IAccountManager interface to include IOrionStartService for additional functionality ([b895d1f](https://www.github.com/tgiachi/Prima/commit/b895d1fd5beece6015fdaf20e1aaa60451cccf8e))
* **INetworkService.cs:** extend INetworkService interface with IDisposable for proper resource management ([56c86ba](https://www.github.com/tgiachi/Prima/commit/56c86ba35df204f8291a42719186be46ccbbe83d))
* **NetworkService.cs:** add INetworkPacketListener interface to handle network packet events ([4c279e7](https://www.github.com/tgiachi/Prima/commit/4c279e7ec3831965734181d2d6ed7ee08bdb0c4b))
* **NetworkService.cs:** refactor NetworkService to handle multiple packets in a single message ([83ea3bf](https://www.github.com/tgiachi/Prima/commit/83ea3bfdcac0a47dc0aeed73039d8844ce97439e))
* **NetworkSession.cs:** add AuthId property to NetworkSession class for authentication ([647f8f3](https://www.github.com/tgiachi/Prima/commit/647f8f3fdf9c0d88e70de0bf328dcffdb73fde44))
* **NetworkSession.cs:** add DisconnectDelegate event to handle session disconnections ([66010f1](https://www.github.com/tgiachi/Prima/commit/66010f1f7da0507c7a822b8945beb0d24ff2db50))
* **NetworkSession.cs:** add event handler for sending packets and properties for Seed and ClientVersion ([9665fc3](https://www.github.com/tgiachi/Prima/commit/9665fc3e60fffb0696f60d1eef8340dc4e8dc0e0))
* **NetworkSession.cs:** add NetworkSession class to handle network sessions ([079e338](https://www.github.com/tgiachi/Prima/commit/079e338b80bfb24cbcb16f5e1b4f07118e70708c))
* **Prima.Core.Server:** add PrimaServerConfig and PrimaServerOptions classes ([f89b686](https://www.github.com/tgiachi/Prima/commit/f89b6867c91e0de5eb16d390e20655a763627b27))
* **Prima.Core.Server:** add PrimaServerContext class to manage service provider and services ([033cfd0](https://www.github.com/tgiachi/Prima/commit/033cfd0d8e17b8dad9022b1c8c7282e39ecb2b00))
* **Prima.Core.Server.csproj:** update Orion.Core.Server package version to 0.15.0 ([2177140](https://www.github.com/tgiachi/Prima/commit/21771401290e06383b325f64200c512e7ebf6f6a))
* **Prima.Network:** add IPacketManager interface for managing network packets ([088fbe8](https://www.github.com/tgiachi/Prima/commit/088fbe8bb58a49997b3e44389278c43b65ae54c6))
* **Prima.sln:** add Prima.Tcp.Test project to the solution file ([8b3d7f4](https://www.github.com/tgiachi/Prima/commit/8b3d7f46bdc6a2a3d8d03ccaef584a8600c48e90))
* **Prima.sln:** rename project "Prima.Network.Packets" to "Prima.Network" ([964075e](https://www.github.com/tgiachi/Prima/commit/964075e73de086293789c6e1ec7652eaa5053cbc))
* **PrimaServerConfig.cs:** add TcpServerConfig class to handle TCP server configuration ([01fc825](https://www.github.com/tgiachi/Prima/commit/01fc825109ef27df1299d833bf0ff2b9370c5fc1))
* **Program.cs:** add MetricsRoute to map metrics-related routes in the application ([b2b1e80](https://www.github.com/tgiachi/Prima/commit/b2b1e809cbafa81f606b08438aee69383cb351cb))
* **server:** add JwtAuthConfig, ShardConfig, AccountServerConfig classes ([e50b578](https://www.github.com/tgiachi/Prima/commit/e50b57816fc9cf87248d45c16e22cde8b3f86f26))
* **sln:** add Prima.Core.Server project to the solution for modularization ([d48e80f](https://www.github.com/tgiachi/Prima/commit/d48e80f7ff6a6ece98a70b6891a92613780f92f1))
* **TcpServerConfig.cs:** add default values for ServerName, LoginPort, and GamePort properties ([6590466](https://www.github.com/tgiachi/Prima/commit/6590466c2b08bab8173b0eb6e5402163208241eb))

### Bug Fixes

* **BasePacketListenerHandler.cs:** update GetSession method in SessionService to include a parameter for checking session existence ([055ff55](https://www.github.com/tgiachi/Prima/commit/055ff558f889710216c5d435fb9fe52a22a2313f))
* **docker_image.yml:** update Docker image names from orionirc-server to prima-server to match project name ([cbba6fe](https://www.github.com/tgiachi/Prima/commit/cbba6fe4d38a1243b34f3cd8d0234245b9772b64))
* **EventLoopConfig.cs:** update TickIntervalMs value from 50 to 90 for slower event loop ticks ([621fc4a](https://www.github.com/tgiachi/Prima/commit/621fc4aea94bfff9904450578f7849e73fd858e0))
* **PacketsTests.cs:** update SessionKey value to a positive number to fix test failure due to negative value being used ([f9d8794](https://www.github.com/tgiachi/Prima/commit/f9d8794673468ecca228b80130dba59de24a457a))

