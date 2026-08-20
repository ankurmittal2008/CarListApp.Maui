# CarListApp.Maui

This is an educational project for the Udemy course called [.NET MAUI Mobile App Development](https://www.udemy.com/course/net-maui-mobile-app-development/?referralCode=A56083F1D67F21799FBB&couponCode=APRIL2023) that allows users to manage a list of cars. The project contains a minimal API project and a .NET MAUI project. There are no API keys needed for this project.

The app uses an SQLite database to store the car data. This project also features a Minimal API example and shows how to integrate a mobile app with an API. 

## Features

- Add, edit, and delete cars from the list
- View detailed information about each car, such as make, model, and year
- Import car data from APIs
- User authenticaiton and authorization
- JWT protection for mobile app
- Cross-platform mobile app

## Technologies Used

- .NET Maui
- SQLite
- Ef Core
- .NET Core minimal API

## Getting Started

To get started with the app, you will need to clone this repository to your local machine:
git clone https://github.com/trevoirwilliams/CarListApp.Maui.git

Then, open the solution file (`CarListApp.Maui.sln`) in Visual Studio and build the solution. The app should launch automatically in the emulator.

## Run migration
```
dotnet tool install --global dotnet-ef
dotnet ef migrations add "initial migration" --project CarListApp.Api\CarListApp.Api.csproj
```

## Signing
```
keytool -genkey -v -keystore carlist.app.keystore -alias key -keyalg RSA -keysize 2048 -validity 10000
dotnet publish -f:net6.0-android -c:Release /p:AndroidSigningKeyPass=P@ssword1 /p:AndroidSigningStorePass=P@ssword1
```

## Contributing

If you would like to contribute to the development of this app, please feel free to submit a pull request. Make sure to follow the [contribution guidelines](CONTRIBUTING.md) when submitting your changes.

## License

This app is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.
