using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Diagnostics; // For Stopwatch and/or debugging later 
using System.Threading;   // For Thread 
//using DataImport;
using System.Reflection.Metadata;


namespace ExamProject
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public string db_ip;
		public string db_port;

		private int _monValue = 0;
		private int _multiValue = 0;
		private int _rpValue = 0;
		private int _ppValue = 0;
		private int _apValue = 0;
		private int _sacValue = 0;
		private int _dietValue = 0;

		// This area is very experimental and i have no idea if it will work.
		static readonly TimeSpan dataUpdateFrequency = TimeSpan.FromMilliseconds(20);

		// Intermittently saves/uploads data / also used to load data
		//private async Task dataThreadProc(char mode = 'S') // modes: S - save U - upload L - load
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
        public int MonValue
        {
            get => _monValue;
            set
            {
                if (_monValue != value)
                {
                    _monValue = value;
                    OnPropertyChanged(nameof(MonValue));
                    Console.WriteLine($"MonValue updated to {_monValue}"); // Debugging statement
                }
            }
        }

        private int MultiValue
		{
			get => _multiValue;
			set
			{
				_multiValue = value;
				OnPropertyChanged(nameof(MultiValue));
			}
		}

		private int RPValue
		{
			get => _rpValue;
			set
			{
				_rpValue = value;
				OnPropertyChanged(nameof(RPValue));
			}
		}

		private int PPValue
		{
			get => _ppValue;
			set
			{
				_ppValue = value;
				OnPropertyChanged(nameof(PPValue));
			}
		}

		private int APValue
		{
			get => _apValue;
			set
			{
				_apValue = value;
				OnPropertyChanged(nameof(APValue));
			}
		}

		private int SacValue
		{
			get => _sacValue;
			set
			{
				_sacValue = value;
				OnPropertyChanged(nameof(SacValue));
			}
		}

		// Implement INotifyPropertiesChanged
		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			Console.WriteLine("Property Changed: {0}", propertyName); 
		}

		// Button Info
		public class ButtonInfo
		{
			public string Name { get; set; }
			public string Tooltip { get; set; }
			public Action? OnClick { get; set; }

			public ButtonInfo(string name, string tooltip, Action? onClick = null)
			{
				Name = name;
				Tooltip = tooltip;
				OnClick = onClick;
			}
		}

		// Section Info
		public class SectionInfo
		{
			public string Header { get; set; }
			public List<ButtonInfo> Buttons { get; set; }
			public SectionInfo(string header, List<ButtonInfo> buttons)
			{
				Header = header;
				Buttons = buttons;
			}
		}

		// Function: Button.Money
        private void Btn_Money(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {

        new SectionInfo("Gain Currency", new List<ButtonInfo>
        {
            new ButtonInfo("Collect Coins",   "Gives you +1 copper.", () => MonValue++)
        }),
        new SectionInfo("Currency Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Basic Copper Mining", "Cost: 1 silver"),

            new ButtonInfo(
            "Enhanced Copper Refinement",
            "Cost: 25 copper.",
            () =>
            {
                const int cost = 25;
                if (MonValue >= cost)
                {
                    MonValue -= cost;
                    // TODO: apply the actual upgrade effect here,
                    // e.g. Increase some production multiplier or level counter.
                }
                else
                {
                    MessageBox.Show("Not enough copper!", "Insufficient Funds",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        ),
            new ButtonInfo("Copper Efficiency",    
			"Cost: 10 copper."),
            new ButtonInfo("Advanced Copper Extraction",    
			"Cost: 10 copper."),
            new ButtonInfo("Copper Amplification System",    
			"Cost: 10 copper."),
            new ButtonInfo("Copper Mega Extraction",    
			"Cost: 10 copper."),
            new ButtonInfo("Galactic Copper Mining",    
			"Cost: 10 copper."),
            new ButtonInfo("Ultimate Copper Extraction",    
			"Cost: 10 copper."),
        }),
        new SectionInfo("Multiplier Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("1.5× Multiplier",   "Cost: 50 copper"),
            new ButtonInfo("3× Multiplier",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

					// Hook up click event!
					Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_Rebirths(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Rebirths", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_Prestiges(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Prestiges", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_Ascentions(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Ascentions", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_Sacrifices(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Sacrifices", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_Deities(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Deities", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_Saints(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Saints", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }

                    stack.Children.Add(btn);
                }

                Grid.SetRow(stack, 1);
                Grid.SetColumn(stack, col);
                content.Children.Add(stack);
            }
        }

        private void Btn_GenStat(object sender, RoutedEventArgs e)
        {
            // Clear previous UI
            content.Children.Clear();
            content.RowDefinitions.Clear();
            content.ColumnDefinitions.Clear();

            // Sample data: 3 sections, each with its own buttons
            var sections = new List<SectionInfo>
    {
        new SectionInfo("Rebirths", new List<ButtonInfo>
        {
            new ButtonInfo("Rebirth!!!",   "Click the button to gain one rebirth."),
        }),
        new SectionInfo("RP Upgrades", new List<ButtonInfo>
        {
            new ButtonInfo("Hyper Growth",   "Cost: 50 copper"),
            new ButtonInfo("Rebirth Efficiency",   "Cost: 1 silver"),
            new ButtonInfo("Recursive Growth",   "Cost: 1 silver"),
            new ButtonInfo("Second Wind",   "Cost: 1 silver"),
            new ButtonInfo("Momentum Stacking",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Overflow",   "Cost: 1 silver"),
            new ButtonInfo("Compounding Returns",   "Cost: 1 silver"),
            new ButtonInfo("Exponential Surge",   "Cost: 1 silver"),
            new ButtonInfo("Legacy Carryover",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Synergy",   "Cost: 1 silver"),
            new ButtonInfo("Energized Rebirths",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Efficiency II",   "Cost: 1 silver"),
            new ButtonInfo("Persistent Momentum",   "Cost: 1 silver"),
            new ButtonInfo("Rebirth Automation",   "Cost: 1 silver"),
        }),
        // …add more sections as you like
    };

            // 3) Build Grid: one column per section
            foreach (var _ in sections)
                content.ColumnDefinitions.Add(new ColumnDefinition());

            // 2 rows: headers + content
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 4) Populate each section
            for (int col = 0; col < sections.Count; col++)
            {
                var section = sections[col];

                // 4a) Header label
                var headerLabel = new TextBlock
                {
                    Text = section.Header,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);
                content.Children.Add(headerLabel);

                // 4b) Container for buttons
                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top
                };

                // 4c) Create each button in this section
                foreach (var info in section.Buttons)
                {
                    var btn = new Button
                    {
                        Content = info.Name,
                        ToolTip = info.Tooltip,
                        Margin = new Thickness(2),
                        Width = 120,
                        Height = 30
                    };

                    // Hook up click event!
                    Console.Write("Event works?");
                    if (info.Name == "Collect Coins")
                    {
                        btn.Click += (s, args) => MonValue += 1;
                        Console.Write("Money Value Increased +N");
                    }
					stack.Children.Add(btn);
				}

				Grid.SetRow(stack, 1);
				Grid.SetColumn(stack, col);
				content.Children.Add(stack);
			}
		}
    }
}
