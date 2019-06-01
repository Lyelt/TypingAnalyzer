using System.Windows.Forms;

namespace TypingAnalyzer
{
    public partial class HostForm : Form
    {
        public HostForm(Analyzer analyzer)
        {
            InitializeComponent();
            analyzer.Start();
        }
    }
}
