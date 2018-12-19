﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tello {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			GamePadManager manager = new GamePadManager();
			manager.Stream += (data) => {
				TelloManager.Instance.Controller = new ControllData(data);
				if (data.A) TelloManager.Instance.TakeOff();
				if (data.B) TelloManager.Instance.Land();
			};
			manager.StartStream();
			TelloManager.Instance.OnConnection += (state) => {
				if (state == ConnectionState.Connected) TelloManager.Instance.SetMaxHeight(5);
				Dispatcher.BeginInvoke(new Action(() => {
					if (state == ConnectionState.Disconnected) (FindName("Connect") as Button).IsEnabled = true;
				}));
			};
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			(FindName("Connect") as Button).IsEnabled = false;
			TelloManager.Instance.Connection();
		}
	}
}
