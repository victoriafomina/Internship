using System.IO;
using System.Windows;
using Antlr4.Runtime;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TestTheDitch();
        }

        public void TestTheDitch()
        {
            ICharStream stream = new AntlrFileStream("..\\..\\text.html");

            // 
            ITokenSource lexer = new HTMLLexer(stream);
            // ITokenStream tokens = new CommonTokenStream(lexer);
        }

    }
}
