FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -r linux-x64 --self-contained false -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# ARG DB_USERS=""
# ARG JWT_SECRET=""
# ARG JWT_ISSUER=""
# ARG JWT_AUDIENCE=""
# ARG HOST_RABBIT=""
# ARG USERNAME_RABBIT=""
# ARG PASSWORD_RABBIT=""

ENV DB_USERS=$DB_USERS
ENV JWT_SECRET=$JWT_SECRET
ENV JWT_ISSUER=$JWT_ISSUER
ENV JWT_AUDIENCE=$JWT_AUDIENCE
ENV HOST_RABBIT=$HOST_RABBIT
ENV USERNAME_RABBIT=$USERNAME_RABBIT
ENV PASSWORD_RABBIT=$PASSWORD_RABBIT

EXPOSE 5089

CMD ["sh", "-c", "dotnet UsersAuthorization.dll"]
