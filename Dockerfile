# 使用官方的 .NET Core SDK 镜像作为基础镜像
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# 复制项目文件并生成发布版
COPY . ./
RUN dotnet publish -c Release -o out

# 构建运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

# 设置容器启动时的命令
ENTRYPOINT ["dotnet", "Ice.Login.Http.dll"]
