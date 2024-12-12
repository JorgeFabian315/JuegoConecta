using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JuegoConecta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int JugadorActual { get; set; } = 1;
        public int Filas { get; set; } = 6;
        public int Columnas { get; set; } = 7;
        public MainWindow()
        {
            InitializeComponent();

            CrearTablero(Columnas, Filas);
        }


        public void CrearTablero(int columnas, int filas)
        {
            tablero.ColumnDefinitions.Clear();
            tablero.RowDefinitions.Clear();


            var styleColumna = CrearEstiloColumnas();

            Color colorBlack = (Color)ColorConverter.ConvertFromString("#2F2F2F"); // Azul hexagonal

            for (int i = 0; i < columnas; i++)
            {
                ColumnDefinition columna = new()
                {
                    Width = new GridLength(1, GridUnitType.Star),
                };

                tablero.ColumnDefinitions.Add(columna);
            }
            for (int i = 0; i < filas; i++)
            {
                RowDefinition fila = new()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };

                tablero.RowDefinitions.Add(fila);
            }

            for (int i = 0; i < columnas; i++)
            {
                for (int j = 0; j < filas; j++)
                {
                    var border = new Border()
                    {

                        Width = 55,
                        Height = 55,
                        Background = Brushes.White,
                        Tag = "ficha"
                    };

                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);


                    tablero.Children.Add(border);
                }

                var borderColumna = new Border()
                {

                    Style = styleColumna,

                };

                if (i == 0)
                    borderColumna.CornerRadius = new CornerRadius(20, 0, 0, 20);
                else if (i == columnas - 1)
                    borderColumna.CornerRadius = new CornerRadius(0, 20, 20, 0);


                Grid.SetColumn(borderColumna, i);
                Grid.SetRowSpan(borderColumna, filas);

                tablero.Children.Add(borderColumna);
            }
        }

        private Style CrearEstiloColumnas()
        {

            Color colorBlack = (Color)ColorConverter.ConvertFromString("#4167E1"); // Azul hexagonal
            Color colorGris = (Color)ColorConverter.ConvertFromString("#90000000"); // Azul hexagonal

            var styleColumna = new Style(typeof(Border));

            styleColumna.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));

            Trigger trigger = new()
            {
                Property = Border.IsMouseOverProperty,
                Value = true
            };

            ColorAnimation colorAnimation = new ColorAnimation
            {
                To = colorGris,     // Color final
                Duration = new Duration(TimeSpan.FromSeconds(0.3)) // Duración de 0.4s
            };

            // Crear un Storyboard y vincular la animación
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);

            // Asociar la animación a la propiedad de fondo
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));


            ColorAnimation exitAnimation = new ColorAnimation
            {
                To = Colors.Transparent, // Volver al color original
                Duration = new Duration(TimeSpan.FromSeconds(0.1))
            };

            // Crear un Storyboard para la animación de salida
            Storyboard exitStoryboard = new Storyboard();
            exitStoryboard.Children.Add(exitAnimation);
            Storyboard.SetTargetProperty(exitAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));


            // Agregar el Storyboard al Trigger
            trigger.EnterActions.Add(new BeginStoryboard { Storyboard = storyboard });
            trigger.ExitActions.Add(new BeginStoryboard { Storyboard = exitStoryboard });
            //trigger.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Gray));

            styleColumna.Triggers.Add(trigger);

            return styleColumna;

        }

        private void tablero_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(tablero);
            int column = (int)(clickPosition.X / (tablero.ActualWidth / Columnas));


            var ultimaFicha = tablero.Children.OfType<Border>()
                .Where(ficha => ficha.Background == Brushes.White
                        && (string)ficha.Tag == "ficha"
                        && Grid.GetColumn(ficha) == column)
                .LastOrDefault();

            SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF474D")); // Naranja


            bool seguirJugando = tablero.Children.OfType<Border>()
                .Any(ficha =>  (string)ficha.Tag == "ficha"
                           && ficha.Background == Brushes.White);

            if(!seguirJugando)
            {
                MessageBox.Show("Fin del juego");
                CrearTablero(Columnas, Filas);
                JugadorActual = 1;
                return;
            }

            if (ultimaFicha == null)
                return;

            if (JugadorActual == 1)
            {
                color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F7D100")); // Amarrillo
                txtjugador.Text = "Jugador 2";
                JugadorActual = 2;
            }
            else
            {
                txtjugador.Text = "Jugador 1";
                JugadorActual = 1;
            }

            txtjugador.Foreground = color;


            ultimaFicha.BorderBrush = Brushes.Black;
            ultimaFicha.BorderThickness = new Thickness(2);
            ultimaFicha.Background = color;
        }



    }
}