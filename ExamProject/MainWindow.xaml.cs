using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Diagnostics; // For Stopwatch and/or debugging later 
using System.Threading;   // For Thread 
//using DataImport;
using System.Reflection.Metadata;


namespace ExamProject
{
	public partial class MainWindow : Window
	{
		public string db_ip;
		public string db_port;

		private int _monValue;
		private int _multiValue;
		private int _rpValue;
		private int _ppValue;
		private int _apValue;
		private int _sacValue;
		private int _dietValue;

		// This area is very experimental and i have no idea if it will work.
		static readonly TimeSpan dataUpdateFrequency = TimeSpan.FromMilliseconds(20);

		// Intermittently saves/uploads data / also used to load data
		//void dataThreadProc(char mode = 'S') // modes: S - save U - upload L - load
		//{
		//	DataRetrieve dataRetrieve = new DataRetrieve();

		//	Dictionary<string, int> valArray = new Dictionary<string, int>();
		//	Stopwatch sw = Stopwatch.StartNew();

		//	while (true)
		//	{
		//		string[] returnedVals = dataRetrieve.getDataFromDB(db_ip, db_port);

		//		// Process each value in returnedVals
		//		for (int i = 0; i < returnedVals.Length; i++)
		//		{

		//			string[] parts = returnedVals[i].Split(':');
		//			if (parts.Length != 2){continue;}

		//			string key = parts[0];
		//			int value;
		//			if (int.TryParse(parts[1], out value))
		//			{
		//				if (valArray.ContainsKey(key))
		//				{
		//					valArray[key] = value;
		//				}
		//				else
		//				{
		//					valArray.Add(key, value);
		//				}
		//			}
		//		}
		//	}
		//}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public MainWindow()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		{
			InitializeComponent();

			// Set the DataContext to this window itself so that the binding in XAML can find these properties.
			DataContext = this;
		}

		// The binds between the lables MonValue and the var _values
		private int MonValue
		{
			get => _monValue;
			set
			{
				_monValue = value;
				OnPropertyChanged();
			}
		}

		private int MultiValue
		{
			get => _multiValue;
			set
			{
				_multiValue = value;
				OnPropertyChanged();
			}
		}

		private int RPValue
		{
			get => _rpValue;
			set
			{
				_rpValue = value;
				OnPropertyChanged();
			}
		}

		private int PPValue
		{
			get => _ppValue;
			set
			{
				_ppValue = value;
				OnPropertyChanged();
			}
		}

		private int APValue
		{
			get => _apValue;
			set
			{
				_apValue = value;
				OnPropertyChanged();
			}
		}

		private int SacValue
		{
			get => _sacValue;
			set
			{
				_sacValue = value;
				OnPropertyChanged();
			}
		}

		private int DietVal
		{
			get => _dietValue;
			set
			{
				_dietValue = value;
				OnPropertyChanged();
			}
		}

		// Implement INotifyPropertiesChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = "null")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		// Example: a button click event that increments values
		private void ScoreButton_Click(object sender, RoutedEventArgs e)
		{
			MonValue += 1;
			MultiValue += 2;
			RPValue += 3;
			APValue += 4; // Hi :3 
			SacValue += 5;
		}


		private void Btn_Money(object sender, RoutedEventArgs e)
		{
            // Clear all children from the Grid
            content.Children.Clear();

            // Optionally, clear row/column definitions if you want to reset layout
            // content.RowDefinitions.Clear();
            // content.ColumnDefinitions.Clear();

            // Example: Adding new items (e.g., Buttons) to the Grid
            for (int i = 0; i < 3; i++)
            {
                Button newButton = new Button
                {
                    Content = $"Button {i + 1}",
                    Margin = new Thickness(5)
                };

                // Set row and column if needed
                Grid.SetRow(newButton, i); // Make sure the grid has enough rows defined
                content.Children.Add(newButton);
            }

        }

	}
}
