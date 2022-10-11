# PUSH_SERVER_WEB

This Repository is made for FrontEnd for [push server](https://github.com/knight7024/go-push-server).

## Start with Blazor WASM!

This FrontEnd has made with Blazor + TS. For Blazor WASM practice, technology stack was chosen as Blazor WASM + TS.

Based on [this repository](https://github.com/cornflourblue/blazor-webassembly-jwt-authentication-example) and [this presentation](https://forum.dotnetdev.kr/t/blazor-webassembly-webpack-sass-typescript-net-conf-2022-x-seoul-replay/3087)

## How To Start

0. Install [.NET 6.0](https://dotnet.microsoft.com/en-us/download) and [nodejs 16.x](https://nodejs.org/en/download/) and [yarn](https://classic.yarnpkg.com/lang/en/docs/install/)
1. Add `appsettings.json` in `PUSH_SERVER_WEB/wwwroot`
   ```json
   {
      "BaseAddress": ""
   }
   ```
2. Put value of BaseAddress(if you don't have, it starts with `FakeBackEndHandler`).
3. Start command `dotnet watch run`.