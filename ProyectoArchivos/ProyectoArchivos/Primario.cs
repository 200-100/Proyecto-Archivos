using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoArchivos
{
    public partial class Primario : Form
    {
       DiccionarioDatos DD = null;

        public Primario()
        {
            InitializeComponent();
        }
        public Primario(DiccionarioDatos dic)
        {
            InitializeComponent();
            DD = dic;
        }
        private void comboSub_BloquePK_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboPK_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Primario_Load(object sender, EventArgs e)
        {

        }
    }
}
