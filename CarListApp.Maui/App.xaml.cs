using CarListApp.Maui.Models;
using CarListApp.Maui.Services;

namespace CarListApp.Maui;

public partial class App : Application
{
	public static UserInfo UserInfo;
	public static CarDatabaseService CarDatabaseService { get; private set; }
	public App(CarDatabaseService carDatabaseService)
	{
		InitializeComponent();
		CarDatabaseService = carDatabaseService;
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		return new Window(new AppShell());
	}
}
