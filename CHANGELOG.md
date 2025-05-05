# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

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

