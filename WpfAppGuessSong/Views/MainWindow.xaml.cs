using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfAppGuessSong.Models;
using WpfAppGuessSong.ViewModels;
using System.Windows.Input;

namespace WpfAppGuessSong.Views
{
    public partial class MainWindow : Window
    {
        private GameViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new GameViewModel();
            this.DataContext = _viewModel;
            MyScrollViewer.RequestBringIntoView += (s, e) => {
                e.Handled = true;
            };
        }

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.BuscarYEmpezar();
        }

        private void BtnReproducir_Click(object sender, RoutedEventArgs e) => _viewModel.Reproducir();

        private void BtnParar_Click(object sender, RoutedEventArgs e) => _viewModel.Detener();

        private void ComboRespuestas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down ||
                e.Key == Key.Enter || e.Key == Key.Tab)
                return;

            var cajaTexto = (TextBox)ComboRespuestas.Template.FindName("PART_EditableTextBox", ComboRespuestas);
            if (cajaTexto == null) return;

            int cursorPosition = cajaTexto.SelectionStart;

            string busquedaOriginal = ComboRespuestas.Text;
            string busquedaNormalizada = Normalizar(busquedaOriginal);

            var vista = CollectionViewSource.GetDefaultView(_viewModel.CancionesRestantes);

            if (string.IsNullOrEmpty(busquedaNormalizada))
            {
                vista.Filter = null;
            }
            else
            {
                vista.Filter = (obj) =>
                {
                    var cancion = obj as Result;
                    if (cancion == null) return false;

                    string nombreCancionNormalizado = Normalizar(cancion.trackName);

                    return nombreCancionNormalizado.Contains(busquedaNormalizada);
                };
            }

            ComboRespuestas.IsDropDownOpen = true;

            cajaTexto.SelectionStart = cursorPosition;
            cajaTexto.SelectionLength = 0;
        }

        private string Normalizar(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;

            var normalizedString = texto.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower();
        }

        private async void BtnSkip_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MostrarPortadaReal = true;
            var tareaSkip = _viewModel.SkipCancion();

            ICollectionView view = CollectionViewSource.GetDefaultView(_viewModel.ListaCanciones);
            view.Refresh();

            await tareaSkip;

            ComboRespuestas.Text = "";
            _viewModel.MostrarPortadaReal = false;
        }

        private async void BtnComprobar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionada = ComboRespuestas.SelectedItem as Result;

            if (seleccionada == null || _viewModel.CancionActual == null) return;

            string Limpiar(string texto) => texto.ToLower()
                .Replace(",", "").Replace(".", "").Replace(" ", "").Trim();

            if (Limpiar(seleccionada.trackName) == Limpiar(_viewModel.CancionActual.trackName))
            {
                _viewModel.MostrarPortadaReal = true;
                _viewModel.FueCorrecto = true;
                _viewModel.Mensaje = "Adivinaste";
                _viewModel.Aciertos++;

                var cancionEnLista = _viewModel.ListaCanciones.FirstOrDefault(c => c.trackName == _viewModel.CancionActual.trackName);
                if (cancionEnLista != null)
                {
                    cancionEnLista.FueAdivinada = true;
                }

                ICollectionView view = CollectionViewSource.GetDefaultView(_viewModel.ListaCanciones);
                view.Refresh();

                await Task.Delay(2000);

                ComboRespuestas.Text = "";
                _viewModel.MostrarPortadaReal = false;
                _viewModel.ActualizarCombo();
                _viewModel.SiguienteRonda();
            }
            else
            {
                _viewModel.FueCorrecto = false;
                _viewModel.Mensaje = "Esa no es... ¡Prueba otra!";
            }
        }
    }
}