/*
*Responsabilidade:
*Armazenar recursos globais, dicionários de estilos e inicializar a janela raiz.
* 
* Papel na arquitetura:
*Disponibiliza conversores de dados (Converters) e cores padronizadas (Styles) 
* para que possam ser consumidos em qualquer arquivo XAML da aplicação sem 
* necessidade de re-declaração.
* 
* Principais recursos globais:
*-Dicionários de Cores e Estilos (Colors.xaml, Styles.xaml).
* - Conversores de visualização (ex: HealthStatusToColorConverter).
*/
namespace KaaDebug
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}