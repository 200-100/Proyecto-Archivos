using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProyectoArchivos
{
    public partial class Form1 : Form
    {
        //Variable de clase DiccionarioDatos que guarda todo el diccionario de datos
        private DiccionarioDatos DD;
        private string nomArchivo;
        private string nomRegistro;

        public Form1()
        {
            InitializeComponent();
            labelProgress.Visible = false;
            TabControl1.Enabled = false;
            comboTipoDato.Items.Add("Entero");
            comboTipoDato.Items.Add("Cadena");
            comboIndice.Items.Add("0 - SIN CLAVE");
            comboIndice.Items.Add("1 - SECUENCIAL");
            comboIndice.Items.Add("2 - SECUANCIAL INDEXADA");
            comboIndice.Items.Add("3 - CLAVE SECUNDARIA");
            comboIndice.Items.Add("5 - HASH ESTATICO");
            txtB_Busqueda.Enabled = false;
        }

        private void button_Agregar_Entidad_Click(object sender, EventArgs e)
        {
            foreach (Entidad enti in DD.Entidades)
            {
                if (string.Compare(Archivo.RellenaNombres(txtB_NomEntidad.Text, 29), enti.Nombre) == 0)
                {
                    MessageBox.Show("Ya existe esa Entidad");
                    return;
                }
            }
            if (!txtB_NomEntidad.Text.Equals(""))
            {
                //SE crea una entidad
                Entidad nuevaEntidad = new Entidad();
                //
                FileInfo file = new FileInfo(nomArchivo);
                //Se crean las celdas para el DataGrid

                nuevaEntidad.Nombre = Archivo.RellenaNombres(txtB_NomEntidad.Text, 29);
                nuevaEntidad.DireccionEntidad = file.Length;
                nuevaEntidad.DireccionAtributo = -1;
                nuevaEntidad.DireccionDato = -1;
                nuevaEntidad.DireccionSigEntidad = -1;

                DD.Entidades.Add(nuevaEntidad);
                Archivo.EscribeEntidad(nuevaEntidad, nomArchivo);

                DD.Entidades = DD.Entidades.OrderBy(entis => entis.Nombre).ToList();
                DD.Entidades = Archivo.ActualizaEntidades(DD.Entidades, nomArchivo);
                dtGrid_Entidades.Rows.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                }
                txtB_NomEntidad.Clear();
                DD.Cabecera = DD.Entidades[0].DireccionEntidad;
                Archivo.EscribeCabecera(DD.Cabecera, nomArchivo);
                txtB_Cabecera.Text = Convert.ToString(DD.Cabecera);

                comboEntidades.Items.Clear();
                comboEntidadesRegistros.Items.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    comboEntidades.Items.Add(en.Nombre);
                    comboEntidadesRegistros.Items.Add(en.Nombre);
                }
            }
        }

        private void button_Modificar_Entidad_Click(object sender, EventArgs e)
        {
            if (DD.Entidades[dtGrid_Entidades.CurrentRow.Index].Registros.Count == 0)
            {
                if (!txtB_NomEntidad.Text.Equals(""))
                {
                    foreach (Entidad en in DD.Entidades)
                    {
                        if (en.Nombre.Equals(dtGrid_Entidades.CurrentCell.Value))
                        {
                            en.Nombre = Archivo.RellenaNombres(txtB_NomEntidad.Text, 29);
                            Archivo.EscribeEntidad(en, nomArchivo);
                        }
                    }
                    DD.Entidades = DD.Entidades.OrderBy(entis => entis.Nombre).ToList();
                    Archivo.ActualizaEntidades(DD.Entidades, nomArchivo);
                    DD.Cabecera = DD.Entidades[0].DireccionEntidad;
                    Archivo.EscribeCabecera(DD.Cabecera, nomArchivo);
                    dtGrid_Entidades.Rows.Clear();
                    foreach (Entidad en in DD.Entidades)
                    {
                        dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                    }
                    txtB_Cabecera.Text = Convert.ToString(DD.Cabecera);
                    comboEntidades.Items.Clear();
                    comboEntidadesRegistros.Items.Clear();
                    foreach (Entidad en in DD.Entidades)
                    {
                        comboEntidades.Items.Add(en.Nombre);
                        comboEntidadesRegistros.Items.Add(en.Nombre);
                    }
                }
            }
            else
            {
                MessageBox.Show("No se puede modificar la entidad porque ya contiene registros.");
            }
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog newFile = new SaveFileDialog();
            newFile.Filter = "Diccionario de Datos|*.dd";
            newFile.Title = "Nuevo";
            newFile.ShowDialog();
            DD = new DiccionarioDatos();

            TabControl1.Enabled = true;
            txtB_Longitud.Enabled = true;
            dtGrid_Entidades.Rows.Clear();
            dtGrid_Atributos.Rows.Clear();
            comboEntidades.Items.Clear();

            DD.Cabecera = -1;
            if (!newFile.FileName.Equals(""))
            {
                nomArchivo = newFile.FileName;
                Archivo.EscribeCabecera(DD.Cabecera, nomArchivo);
                TabControl1.Enabled = true;
                txtB_Cabecera.Text = Convert.ToString(DD.Cabecera);
            }
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            long dirAux = 0;
            Entidad entidadLeida = null;
            Registro registroLeido = null;
            IndicePrimario indiceprimarioLeido = null;
            IndiceSecundario indicesecundarioLeido = null;
            List<HashEstatico> hashEstatico = null;
            HashDinamico hsLeido = null;
            string rutaReg, rutaIdx;
            FileInfo file;

            if (open.ShowDialog() == DialogResult.OK)
            {
                nomArchivo = open.FileName;
                open.Filter = "Diccionario de Datos|*.dd";
                open.Title = "Abrir";
                DD = new DiccionarioDatos();
                FileInfo f;

                TabControl1.Enabled = true;
                DD.Cabecera = Archivo.LeeCabezera(nomArchivo);
                txtB_Cabecera.Text = Convert.ToString(DD.Cabecera);
                dirAux = DD.Cabecera;
                dtGrid_Entidades.Rows.Clear();
                while (dirAux != -1)
                {
                    entidadLeida = Archivo.LeeEntidad(dirAux, nomArchivo);
                    DD.Entidades.Add(entidadLeida);
                    dirAux = entidadLeida.DireccionSigEntidad;
                }
                dtGrid_Entidades.Rows.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    nomRegistro = Path.GetDirectoryName(nomArchivo);
                    rutaReg = nomRegistro + @"\" + en.Nombre + ".dat";
                    f = new FileInfo(rutaReg);
                    if (!f.Exists)
                    {
                        en.DireccionDato = -1;
                        Archivo.EscribeEntidad(en, nomArchivo);
                    }
                    nomRegistro = Path.GetDirectoryName(nomArchivo);
                    rutaReg = nomRegistro + @"\" + en.Nombre + ".idx";
                    f = new FileInfo(rutaReg);
                    if (!f.Exists)
                    {
                        foreach (Atributo at in en.Atributos)
                        {
                            at.DireccionIndice = -1;
                            Archivo.EscribeAtributo(at, nomArchivo);
                        }
                    }
                    dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                    TabControl1.Enabled = true;
                }
                comboEntidadesRegistros.Items.Clear();
                comboEntidades.Items.Clear();
                comboPK.Items.Clear();
                comboFK.Items.Clear();
                comboHashDinamico.Items.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    comboEntidades.Items.Add(en.Nombre);
                    comboEntidadesRegistros.Items.Add(en.Nombre);
                    comboPK.Items.Add(en.Nombre);
                    comboHashDinamico.Items.Add(en.Nombre);
                    comboHashEstatico.Items.Add(en.Nombre);
                }
                dtGrid_Registros.Rows.Clear();
                dtGrid_Registros.Columns.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    nomRegistro = Path.GetDirectoryName(nomArchivo);
                    rutaReg = nomRegistro + @"\" + en.Nombre + ".dat";
                    file = new FileInfo(rutaReg);

                    comboFK.Items.Add(en.Nombre);
                    if (file.Exists)
                    {
                        dirAux = en.DireccionDato;
                        while (dirAux != -1)
                        {
                            registroLeido = Archivo.LeeRegistro(dirAux, en, rutaReg);
                            en.Registros.Add(registroLeido);
                            dirAux = registroLeido.DirSigRegistro;
                        }
                    }
                    rutaIdx = nomRegistro + @"\" + en.Nombre + ".idx";
                    file = new FileInfo(rutaIdx);
                    if (file.Exists)
                    {
                        foreach (Atributo at in en.Atributos)
                        {
                            if (at.DireccionIndice != -1)
                            {
                                dirAux = at.DireccionIndice;
                                switch (at.TipoIndice)
                                {
                                    case 2:
                                        #region Indice Primario
                                        if (at.TipoDato == 'E')
                                        {
                                            for (int i = 0; i < 9; i++)
                                            {
                                                indiceprimarioLeido = Archivo.LeeIndicePrimario(dirAux, at, rutaIdx);
                                                if (indiceprimarioLeido.DirBloque != -1)
                                                {
                                                    indiceprimarioLeido.Sub_Bloque = Archivo.LeeSub_BloqueIndicePrimario(indiceprimarioLeido.DirBloque, at, rutaIdx);
                                                }
                                                en.PKs.Add(indiceprimarioLeido);
                                                dirAux += 12;
                                            }
                                        }
                                        else if (at.TipoDato == 'C')
                                        {
                                            for (int i = 0; i < 26; i++)
                                            {
                                                indiceprimarioLeido = Archivo.LeeIndicePrimario(dirAux, at, rutaIdx);
                                                if (indiceprimarioLeido.DirBloque != -1)
                                                {
                                                    indiceprimarioLeido.Sub_Bloque = Archivo.LeeSub_BloqueIndicePrimario(indiceprimarioLeido.DirBloque, at, rutaIdx);
                                                }
                                                en.PKs.Add(indiceprimarioLeido);
                                                dirAux += 9;
                                            }
                                        }
                                        break;
                                    #endregion
                                    case 3:
                                        #region Indice Secundario
                                        List<IndiceSecundario> auxSec = new List<IndiceSecundario>();

                                        comboAtributosFK.Items.Add(at.Nombre);
                                        if (at.TipoDato == 'E')
                                        {
                                            for (int i = 0; i < 50; i++)
                                            {
                                                indicesecundarioLeido = Archivo.LeeIndiceSecundario(dirAux, at, rutaIdx);
                                                comboSub_BloqueFK.Items.Add(indicesecundarioLeido.ID);
                                                if (indicesecundarioLeido.DirBloque != -1)
                                                {
                                                    indicesecundarioLeido.Sub_Bloque = Archivo.LeeSub_BloqueIndiceSecundario(indicesecundarioLeido.DirBloque, at, rutaIdx);
                                                }
                                                auxSec.Add(indicesecundarioLeido);
                                                dirAux += 12;
                                            }
                                            en.FKs.Add(auxSec);
                                        }
                                        else if (at.TipoDato == 'C')
                                        {
                                            for (int i = 0; i < 50; i++)
                                            {
                                                indicesecundarioLeido = Archivo.LeeIndiceSecundario(dirAux, at, rutaIdx);
                                                comboSub_BloqueFK.Items.Add(indicesecundarioLeido.ID);
                                                if (indicesecundarioLeido.DirBloque != -1)
                                                {
                                                    indicesecundarioLeido.Sub_Bloque = Archivo.LeeSub_BloqueIndiceSecundario(indicesecundarioLeido.DirBloque, at, rutaIdx);
                                                }
                                                auxSec.Add(indicesecundarioLeido);
                                                dirAux += at.Longitud + 8;
                                            }
                                            en.FKs.Add(auxSec);
                                        }
                                        break;
                                    #endregion
                                    case 5:
                                        #region Hash Estatico
                                        if (at.TipoDato == 'C')
                                        {
                                            hashEstatico = Archivo.LeeHashEstaticoBloque(at.DireccionIndice, rutaIdx);
                                            foreach (HashEstatico h in hashEstatico)
                                            {
                                                h.Sub_Bloque = Archivo.LeeHashHashEstaticoSubBloque(h.DirBloque, at, rutaIdx);
                                                h.Desbordamiento = Archivo.LeeHashEstaticoDesbordamiento(h.Sub_Bloque[h.Sub_Bloque.Count - 1].DirSubBloque + at.Longitud + 8, rutaIdx);
                                            }
                                            en.HE = hashEstatico;
                                        }
                                        break;
                                    #endregion
                                    case 6:
                                        #region HashDinamico
                                        hsLeido = Archivo.LeeHashDinamicoBloque(dirAux, rutaIdx);
                                        for (int i = 0; i < 64; i++)
                                        {
                                            HashDinamico_SubBloque aux;
                                            if (hsLeido.BloquePrincipal[i].DirSub_Bloque != -1)
                                            {
                                                hsLeido.BloquePrincipal[i].Sub_Bloque = Archivo.LeeHashDinamicoSubBloque(hsLeido.BloquePrincipal[i].DirSub_Bloque, rutaIdx);
                                                aux = hsLeido.BloquePrincipal[i].Sub_Bloque;
                                                while (aux.DirDesbordamiento != -1)
                                                {
                                                    aux.ListaDesbordamiento = Archivo.LeeHashDinamicoSubBloque(aux.DirDesbordamiento, rutaIdx);
                                                    aux = aux.ListaDesbordamiento;
                                                }

                                            }
                                        }
                                        en.HS = hsLeido;
                                        break;
                                        #endregion

                                }
                            }
                        }
                    }
                }
            }
            else
                return;
        }

        private void button_Agregar_Atributo_Click(object sender, EventArgs e)
        {
            Atributo nuevoAtributo = new Atributo();
            FileInfo file = new FileInfo(nomArchivo);
            int posEntidad = comboEntidades.SelectedIndex;

            if (!txtB_NomAtributo.Text.Equals("") && comboEntidades.SelectedIndex != -1 && comboTipoDato.SelectedIndex != -1 && comboIndice.SelectedIndex != -1 && !txtB_Longitud.Text.Equals(""))
            {
                foreach (Atributo at in DD.Entidades[posEntidad].Atributos)
                {
                    if (string.Compare(Archivo.RellenaNombres(txtB_NomAtributo.Text, 29), at.Nombre) == 0)
                    {
                        MessageBox.Show("Ya existe ese Atributo");
                        return;
                    }
                }
                foreach (Atributo at in DD.Entidades[posEntidad].Atributos)
                {
                    if (at.TipoIndice == 1 && comboIndice.SelectedIndex == 1)
                    {
                        MessageBox.Show("Ya existe una clave de busqueda");
                        return;
                    }
                }
                foreach (Atributo at in DD.Entidades[posEntidad].Atributos)
                {
                    if (at.TipoIndice == 2 && comboIndice.SelectedIndex == 2)
                    {
                        MessageBox.Show("Ya existe un indice primario");
                        return;
                    }
                }
                nuevoAtributo.Nombre = Archivo.RellenaNombres(txtB_NomAtributo.Text, 29);
                nuevoAtributo.DireccionAtributo = file.Length;
                if (comboRelacion.SelectedIndex > -1)
                {
                    nuevoAtributo.TablaRelacion = Archivo.RellenaNombres(comboRelacion.Items[comboRelacion.SelectedIndex].ToString(), 15);
                }
                else if (comboRelacion.SelectedIndex == -1)
                {
                    nuevoAtributo.TablaRelacion = Archivo.RellenaNombres("----------", 29);
                }
                if (comboTipoDato.SelectedIndex == 0)
                {
                    nuevoAtributo.TipoDato = 'E';
                    nuevoAtributo.Longitud = 4;
                    txtB_Longitud.Text = Convert.ToString(nuevoAtributo.Longitud);
                }
                else
                {
                    nuevoAtributo.TipoDato = 'C';
                    nuevoAtributo.Longitud = Convert.ToInt32(txtB_Longitud.Text);
                }
                switch (comboIndice.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        nuevoAtributo.TipoIndice = Convert.ToInt32(comboIndice.SelectedIndex);
                        break;
                    case 4:
                        nuevoAtributo.TipoIndice = Convert.ToInt32(comboIndice.SelectedIndex + 1);
                        break;

                }
                nuevoAtributo.DireccionIndice = -1;
                nuevoAtributo.DireccionSigAtributo = -1;
                DD.Entidades[posEntidad].Atributos.Add(nuevoAtributo);
                Archivo.EscribeAtributo(nuevoAtributo, nomArchivo);
                DD.Entidades[posEntidad].Atributos = Archivo.ActualizaAtributos(DD.Entidades[posEntidad].Atributos, nomArchivo);
                DD.Entidades = Archivo.ActualizaEntidades(DD.Entidades, nomArchivo);
                dtGrid_Atributos.Rows.Clear();
                foreach (Atributo at in DD.Entidades[comboEntidades.SelectedIndex].Atributos)
                {
                    dtGrid_Atributos.Rows.Add(at.Nombre, at.DireccionAtributo, at.TipoDato, at.Longitud, at.TipoIndice, at.TablaRelacion, at.DireccionIndice, at.DireccionSigAtributo);
                }
                dtGrid_Entidades.Rows.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                    TabControl1.Enabled = true;
                }
                comboEntidades.Text = "";
                comboIndice.Text = "";
                comboTipoDato.Text = "";
                txtB_Longitud.Text = "";
                txtB_Longitud.Enabled = true;
                txtB_NomAtributo.Clear();
            }
            else
            {
                MessageBox.Show("Toda la información no esta completa");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {//Modifia atributo
            string nuevoNombre = txtB_NomAtributo.Text;

            if (!nuevoNombre.Equals("") && !txtB_Longitud.Text.Equals(""))
            {
                foreach (Atributo at in DD.Entidades[comboEntidades.SelectedIndex].Atributos)
                {
                    if (string.Compare(Archivo.RellenaNombres(txtB_NomAtributo.Text, 29), at.Nombre) == 0)
                    {
                        MessageBox.Show("Ya existe ese Atributo");
                        return;
                    }
                }
                foreach (Atributo at in DD.Entidades[comboEntidades.SelectedIndex].Atributos)
                {
                    if (at.TipoIndice == 1 && comboIndice.SelectedIndex == 1)
                    {
                        MessageBox.Show("Ya existe un indice primario");
                        return;
                    }
                    else if (at.TipoIndice == 2 && comboIndice.SelectedIndex == 2)
                    {
                        MessageBox.Show("Ya existe un indice secundario");
                        return;
                    }
                }
                foreach (Entidad en in DD.Entidades)
                {
                    foreach (Atributo at in en.Atributos)
                    {
                        if (at.Nombre.Equals(dtGrid_Atributos.CurrentCell.Value))
                        {
                            at.Nombre = Archivo.RellenaNombres(nuevoNombre, 29);
                            if (comboTipoDato.SelectedIndex == 0)
                            {
                                at.TipoDato = 'E';
                                at.Longitud = 4;
                                txtB_Longitud.Text = Convert.ToString(at.Longitud);
                            }
                            else
                            {
                                at.TipoDato = 'C';
                                at.Longitud = Convert.ToInt32(txtB_Longitud.Text);
                            }
                            at.TipoIndice = Convert.ToInt32(comboIndice.SelectedIndex);
                            Archivo.EscribeAtributo(at, nomArchivo);
                        }
                    }
                }
            }
            dtGrid_Atributos.Rows.Clear();
            foreach (Atributo at in DD.Entidades[comboEntidades.SelectedIndex].Atributos)
            {
                dtGrid_Atributos.Rows.Add(at.Nombre, at.DireccionAtributo, at.TipoDato, at.Longitud, at.TipoIndice, at.TablaRelacion, at.DireccionIndice, at.DireccionSigAtributo);
            }
            txtB_NomEntidad.Text = "";
            txtB_Longitud.Text = "";
            txtB_Longitud.Enabled = true;
            comboEntidades.Text = "";
            comboIndice.Text = "";
            comboTipoDato.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DD.Entidades[dtGrid_Entidades.CurrentRow.Index].Registros.Count == 0)
            {
                DD.Entidades.RemoveAt(dtGrid_Entidades.CurrentRow.Index);
                Archivo.ActualizaEntidades(DD.Entidades, nomArchivo);
                dtGrid_Entidades.Rows.Clear();
                comboEntidades.Items.Clear();
                comboEntidadesRegistros.Items.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                    comboEntidades.Items.Add(en.Nombre);
                    comboEntidadesRegistros.Items.Add(en.Nombre);
                }
                if (DD.Entidades.Count > 0)
                {
                    DD.Cabecera = DD.Entidades[0].DireccionEntidad;
                    txtB_Cabecera.Text = Convert.ToString(DD.Cabecera);
                    Archivo.EscribeCabecera(DD.Cabecera, nomArchivo);
                }
                else
                {
                    DD.Cabecera = -1;
                    txtB_Cabecera.Text = Convert.ToString(DD.Cabecera);
                    Archivo.EscribeCabecera(DD.Cabecera, nomArchivo);
                }
            }
            else
            {
                MessageBox.Show("No se puede eliminar está entidad porque ya contiene registros.");
            }
        }

        private void comboTipoDato_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboTipoDato.SelectedIndex == 0)
            {
                txtB_Longitud.Text = "4";
                txtB_Longitud.Enabled = false;
            }
            else if (comboTipoDato.SelectedIndex == 1)
            {
                txtB_Longitud.Text = "";
                txtB_Longitud.Enabled = true;
            }
        }

        private void comboEntidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtGrid_Atributos.Rows.Clear();
            FileInfo f = new FileInfo(nomRegistro + @"\" + DD.Entidades[comboEntidades.SelectedIndex].Nombre + ".dat");

            //button_Agregar_Atributo.Enabled = button_Modifica_Atributo.Enabled = button_Elimina_Atributo.Enabled = !f.Exists;
            foreach (Atributo at in DD.Entidades[comboEntidades.SelectedIndex].Atributos)
            {
                dtGrid_Atributos.Rows.Add(at.Nombre, at.DireccionAtributo, at.TipoDato, at.Longitud, at.TipoIndice, at.TablaRelacion, at.DireccionIndice, at.DireccionSigAtributo);
            }

            button_Agregar_Atributo.Enabled = button_Modifica_Atributo.Enabled = button_Elimina_Atributo.Enabled = DD.Entidades[comboEntidades.SelectedIndex].Registros.Count == 0;

        }

        private void button_Elimina_Atributo_Click(object sender, EventArgs e)
        {
            DD.Entidades[comboEntidades.SelectedIndex].Atributos.RemoveAt(dtGrid_Atributos.CurrentRow.Index);
            Archivo.ActualizaAtributos(DD.Entidades[comboEntidades.SelectedIndex].Atributos, nomArchivo);
            Archivo.ActualizaEntidades(DD.Entidades, nomArchivo);
            dtGrid_Entidades.Rows.Clear();
            foreach (Entidad en in DD.Entidades)
            {
                dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
            }
            dtGrid_Atributos.Rows.Clear();
            foreach (Atributo at in DD.Entidades[comboEntidades.SelectedIndex].Atributos)
            {
                dtGrid_Atributos.Rows.Add(at.Nombre, at.DireccionAtributo, at.TipoDato, at.Longitud, at.TipoIndice, at.TablaRelacion, at.DireccionIndice, at.DireccionSigAtributo);
            }
            txtB_Longitud.Enabled = true;
            txtB_Longitud.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nomRegistro = Path.GetDirectoryName(nomArchivo);
            FileInfo f = new FileInfo(nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".dat");

            if (f.Exists)
            {

                button_DarAlta_Registro.Enabled = false;
                dtGrid_Registros.Rows.Clear();
                dtGrid_Registros.Columns.Clear();
                dtGrid_Registros.Columns.Add("CDirReg", "Dirección del Registro");

                foreach (Atributo at in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos)
                {
                    dtGrid_Registros.Columns.Add(at.Nombre, at.Nombre);
                }
                dtGrid_Registros.Columns.Add("CDirSigReg", "Dirección del Siguiente Registro");
                dtGrid_Registros.Rows.Clear();
                foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
                {
                    dtGrid_Registros.Rows.Add();
                }
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
                {
                    dtGrid_Registros[0, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirRegistro;
                    dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirSigRegistro;
                }
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
                {
                    for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                    {
                        dtGrid_Registros[j, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].Informacion[j - 1];
                    }
                }
                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count != 0)
                {
                    foreach (Atributo at in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos)
                    {
                        comboBusqueda.Items.Add(at.Nombre);
                    }
                    txtB_Busqueda.Enabled = false;
                }
            }
            else
            {
                button_DarAlta_Registro.Enabled = true;
                dtGrid_Registros.Rows.Clear();
                dtGrid_Registros.Columns.Clear();
            }
        }

        private void button_DarAlta_Registro_Click(object sender, EventArgs e)
        {
            #region Declaracion de variables
            string nameFileDat = "";
            string nameFileIdx = "";
            FileInfo f;
            long tam;
            #endregion

            if (comboEntidadesRegistros.SelectedIndex != -1)
            {
                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count > 0)
                {
                    dtGrid_Registros.Columns.Add("CDirReg", "Dirección del Registro");
                    foreach (Atributo at in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos)
                    {
                        dtGrid_Registros.Columns.Add(at.Nombre, at.Nombre);
                    }
                    dtGrid_Registros.Columns.Add("CDirSigReg", "Dirección del Siguiente Registro");

                    nomRegistro = Path.GetDirectoryName(nomArchivo);
                    nameFileDat = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".dat";
                    nameFileIdx = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx";
                    Archivo.CreaArchivoDat(nameFileDat);
                    Archivo.CreaArchivoIdx(nameFileIdx);
                    foreach (Atributo at in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos)
                    {
                        switch (at.TipoIndice)
                        {
                            case 2:
                                #region Indice Primario
                                f = new FileInfo(nameFileIdx);
                                tam = f.Length;
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs = Archivo.InicializaIndicePrimario(tam, at, nameFileIdx);
                                comboPK.Items.Clear();
                                foreach (Entidad en in DD.Entidades)
                                {
                                    comboPK.Items.Add(en.Nombre);
                                }
                                at.DireccionIndice = tam;
                                Archivo.EscribeAtributo(at, nomArchivo);
                                comboEntidades.Text = "";
                                comboPK.Text = "";
                                comboSub_BloquePK.Text = "";
                                dtGrid_Atributos.Rows.Clear();
                                dtGrid_PK.Rows.Clear();
                                dtGrid_Sub_BloquesPK.Rows.Clear();
                                break;
                            #endregion
                            case 3:
                                #region Indice Secundario
                                f = new FileInfo(nameFileIdx);
                                tam = f.Length;
                                List<IndiceSecundario> auxIS = Archivo.InicializaIndiceSecundario(tam, at, nameFileIdx);
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs.Add(auxIS);
                                comboFK.Items.Clear();
                                foreach (Entidad en in DD.Entidades)
                                {
                                    comboFK.Items.Add(en.Nombre);
                                }
                                at.DireccionIndice = tam;
                                Archivo.EscribeAtributo(at, nomArchivo);
                                comboEntidades.Text = "";
                                comboFK.Text = "";
                                comboAtributosFK.Text = "";
                                comboSub_BloqueFK.Text = "";
                                dtGrid_Atributos.Rows.Clear();
                                dtGrid_FK.Rows.Clear();
                                break;
                            #endregion
                            case 5:
                                f = new FileInfo(nameFileIdx);
                                tam = f.Length;
                                at.DireccionIndice = tam;
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE = Archivo.InicializaHashEstatico(tam, nameFileIdx);
                                foreach (HashEstatico hesb in DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE)
                                {
                                    f = new FileInfo(nameFileIdx);
                                    tam = f.Length;
                                    hesb.DirBloque = tam;
                                    hesb.Sub_Bloque = Archivo.InicializaSub_BloqueInicializaHashEstatico(tam, at, hesb, nameFileIdx);
                                }
                                Archivo.EscribeHashEstaticoBloque(at.DireccionIndice, at, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE, nameFileIdx);
                                for (int h = 1; h <= 10; h++)
                                {
                                    comboCajon.Items.Add("L" + h.ToString());
                                }
                                comboHashEstatico.Items.Clear();
                                foreach (Entidad en in DD.Entidades)
                                {
                                    comboHashEstatico.Items.Add(en.Nombre);
                                }
                                Archivo.EscribeAtributo(at, nomArchivo);
                                dtGrid_Atributos.Rows.Clear();
                                break;
                            #region Hash Dinamico
                            case 6:
                                #region Hash Dinamico
                                f = new FileInfo(nameFileIdx);
                                tam = f.Length;
                                at.DireccionIndice = tam;
                                Archivo.EscribeAtributo(at, nomArchivo);
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS = Archivo.InicializaHashDinamico(tam, nameFileIdx);
                                f = new FileInfo(nameFileIdx);
                                tam = f.Length;
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[0].Sub_Bloque = Archivo.InicializaHashDinamicoSubBloque(tam, nameFileIdx);
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[0].DirSub_Bloque = tam;
                                comboHashDinamico.Items.Clear();
                                foreach (Entidad en in DD.Entidades)
                                {
                                    comboHashDinamico.Items.Add(en.Nombre);

                                }
                                dtGrid_Atributos.Rows.Clear();
                                dtGrid_HashDinamico.Rows.Clear();
                                dtGrid_HashDinamicoSub_Bloque.Rows.Clear();
                                txtB_IDHashDinamico.Text = "";
                                txtB_IDHashDinamicoSub_Bloque.Text = "";
                                break;
                                #endregion
                            #endregion
                        }
                    }
                    dtGrid_Registros.Rows.Clear();
                    comboEntidadesRegistros.Text = "";
                }
                else
                {
                    MessageBox.Show("Aún no hay atributos para los registros");
                }
            }
        }


        private void button_Agregar_Registro_Click(object sender, EventArgs e)
        {
            try
            {

                #region Declaracion de variables locales
                Registro r = new Registro();
                string rutaReg = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".dat";
                FileInfo file = new FileInfo(rutaReg);
                char cAux;
                int iAux;
                string sAux;
                long tam;
                bool vacio = true;
                int pos = 0;
                int posE = comboEntidadesRegistros.SelectedIndex;
                int tRelacion = -1;
                int rPrimario = -1;
                int rSecundario = -1;
                int cont = 0;
                #endregion

                #region Registros
                r.DirRegistro = (long)file.Length;
                for (int i = 1; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; i++)
                {
                    r.Informacion.Add(Archivo.RellenaNombres(Convert.ToString(dtGrid_Registros.CurrentRow.Cells[i].Value), DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i - 1].Longitud - 1));
                }
                r.DirSigRegistro = -1;
                #region Busqueda de Informacion de Indice Primario en Indice Secundario
                for (int i = 0; i < DD.Entidades[posE].Atributos.Count; i++)
                {
                    cont = 0;
                    if (DD.Entidades[posE].Atributos[i].TipoIndice.Equals(3))
                    {
                        rSecundario = i;
                        for (int j = 0; j < DD.Entidades.Count; j++)
                        {
                            if (DD.Entidades[posE].Atributos[i].TablaRelacion.Equals(DD.Entidades[j].Nombre))
                            {
                                tRelacion = j;
                                for (int k = 0; k < DD.Entidades[tRelacion].Atributos.Count; k++)
                                {
                                    if (DD.Entidades[tRelacion].Atributos[k].TipoIndice.Equals(2))
                                    {
                                        rPrimario = k;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        if (tRelacion != -1)
                        {
                            if (DD.Entidades[tRelacion].Registros.Count > 0)
                            {
                                for (int o = 0; o < DD.Entidades[tRelacion].Registros.Count; o++)
                                {
                                    if ((DD.Entidades[tRelacion].Atributos[rSecundario].TipoDato.Equals('E')))
                                    { 
                                        if (r.Informacion[rSecundario].Equals(Archivo.RellenaNombres(Convert.ToString(DD.Entidades[tRelacion].Registros[o].Informacion[rPrimario]), 3))) { break; }
                                        else { cont++; }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se puede insetar el registro porque no hay registros para que haya una relacion con la clave secundaria.");
                                return;
                            }
                            if (cont == DD.Entidades[tRelacion].Registros.Count)
                            {
                                MessageBox.Show("No se puede insertar el registro porque la informacion de la clave secundaria no coincide con la de la clave primaria en la tabla con la que hace relación.");
                                dtGrid_Registros.Rows.Clear();
                                foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
                                {
                                    dtGrid_Registros.Rows.Add();
                                }
                                for (int k = 0; k < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; k++)
                                {
                                    dtGrid_Registros[0, k].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[k].DirRegistro;
                                    dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, k].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[k].DirSigRegistro;
                                }
                                for (int k = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; k++)
                                {
                                    for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                                    {
                                        dtGrid_Registros[j, k].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[k].Informacion[j - 1];
                                    }
                                }
                                return;
                            }
                        }
                    }
                }
                #endregion
                #region Busqueda de que no se repitan los Indices Primario
                for (int i = 0; i < DD.Entidades[posE].Atributos.Count; i++)
                {
                    if (DD.Entidades[posE].Atributos[i].TipoIndice.Equals(2))
                    {
                        rPrimario = i;
                        dtGrid_Registros.Rows.Clear();
                        foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
                        {
                            dtGrid_Registros.Rows.Add();
                        }
                        for (int k = 0; k < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; k++)
                        {
                            dtGrid_Registros[0, k].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[k].DirRegistro;
                            dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, k].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[k].DirSigRegistro;
                        }
                        for (int k = 0; k < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; k++)
                        {
                            for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                            {
                                dtGrid_Registros[j, k].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[k].Informacion[j - 1];
                            }
                        }
                        break;
                    }
                }
                for (int Y = 0; Y < DD.Entidades[posE].Atributos.Count; Y++)
                {
                    if (DD.Entidades[posE].Atributos[Y].TipoIndice.Equals(2))
                    {
                        for (int i = 0; i < DD.Entidades[posE].Registros.Count; i++)
                        {
                            if (Archivo.RellenaNombres(Convert.ToString(DD.Entidades[posE].Registros[i].Informacion[rPrimario]), 3) == Archivo.RellenaNombres(Convert.ToString(r.Informacion[rPrimario]), 3))
                            {
                                MessageBox.Show("No se puede almacenar la informacion porque ya existe algún registro con la misma clave primaria.");
                                return;
                            }
                        }
                    }
                }
                #endregion
                Archivo.EscribeRegistro(r, DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
                DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Add(r);
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
                {
                    if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice == 1 && DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                    {
                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.OrderBy(reg => reg.Informacion[i]).ToList();
                    }
                    else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice == 1 && DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                    {
                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.OrderBy(reg => reg.Informacion[i]).ToList();
                    }
                }
                DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = Archivo.ActualizaRegistros(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros, DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
                DD.Entidades[comboEntidadesRegistros.SelectedIndex].DireccionDato = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[0].DirRegistro;
                Archivo.EscribeEntidad(DD.Entidades[comboEntidadesRegistros.SelectedIndex], nomArchivo);
                dtGrid_Entidades.Rows.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                }
                dtGrid_Registros.Rows.Clear();
                foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
                {
                    dtGrid_Registros.Rows.Add();
                }
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
                {
                    dtGrid_Registros[0, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirRegistro;
                    dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirSigRegistro;
                }
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
                {
                    for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                    {
                        dtGrid_Registros[j, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].Informacion[j - 1];
                    }
                }
                #endregion
                #region Indices
                //Aquí comienza el rollo de indices :(
                rutaReg = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx";
                file = new FileInfo(rutaReg);
                pos = 0;
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
                {
                    //Indice Primario
                    switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice)
                    {
                        case 2:
                            #region Indice Primario
                            file = new FileInfo(rutaReg);
                            switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                            {
                                case 'E':
                                    sAux = Convert.ToString(r.Informacion[i]);
                                    iAux = Convert.ToInt32(sAux[0]) - 48;
                                    foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                                    {
                                        if (iAux == Convert.ToChar(ip.ID))
                                        {
                                            if (ip.DirBloque == -1)
                                            {//No hay ninguno, osea que será el primero
                                                tam = (long)file.Length;
                                                ip.DirBloque = tam;
                                                ip.Sub_Bloque = Archivo.InicializaSub_BloqueIndicePrimario(ip.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                                {
                                                    if (Convert.ToInt32(ips.Informacion) == 0)
                                                    {
                                                        ips.Informacion = Convert.ToInt32(r.Informacion[i]);
                                                        ips.DirInformacion = r.DirRegistro;
                                                        break;
                                                    }
                                                }
                                                Archivo.EscribeIndicePrimario(ip, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            }
                                            else
                                            {//Ya no es el primero
                                                foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                                {
                                                    if (Convert.ToInt32(ips.Informacion) == 0)
                                                    {
                                                        ips.Informacion = Convert.ToInt32(r.Informacion[i]);
                                                        ips.DirInformacion = r.DirRegistro;
                                                        break;
                                                    }
                                                }
                                                ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                                Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);

                                            }
                                        }
                                    }
                                    break;
                                case 'C':
                                    sAux = Convert.ToString(r.Informacion[i]);
                                    cAux = Convert.ToChar(sAux[0]);
                                    foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                                    {
                                        if (cAux == Convert.ToChar(ip.ID))
                                        {
                                            if (ip.DirBloque == -1)
                                            {//No hay ninguno, osea que será el primero
                                                tam = (long)file.Length;
                                                ip.DirBloque = tam;
                                                ip.Sub_Bloque = Archivo.InicializaSub_BloqueIndicePrimario(ip.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                                {
                                                    if (Convert.ToString(ips.Informacion).Equals(Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1)))
                                                    {
                                                        ips.Informacion = Convert.ToString(r.Informacion[i]);
                                                        ips.DirInformacion = r.DirRegistro;
                                                        break;
                                                    }
                                                }
                                                Archivo.EscribeIndicePrimario(ip, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            }
                                            else
                                            {//Ya no es el primero
                                                foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                                {
                                                    if (Convert.ToString(ips.Informacion).Equals(Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1)))
                                                    {
                                                        ips.Informacion = Convert.ToString(r.Informacion[i]);
                                                        ips.DirInformacion = r.DirRegistro;
                                                        break;
                                                    }
                                                }
                                                ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                                Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);

                                            }
                                        }
                                    }
                                    comboPK.Text = "";
                                    comboSub_BloquePK.Text = "";
                                    dtGrid_PK.Rows.Clear();
                                    dtGrid_Sub_BloquesPK.Rows.Clear();
                                    break;
                            }
                            break;
                        #endregion
                        case 3:
                            #region Indice Secundario
                            file = new FileInfo(rutaReg);
                            switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                            {
                                case 'E':
                                    foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                    {
                                        if (Convert.ToInt32(iS.ID) != 0)
                                        {
                                            vacio = false;
                                            break;
                                        }
                                    }
                                    if (vacio)//Esta vacio el bloque de indice secundario, entra el primero
                                    {
                                        foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                        {
                                            if (Convert.ToInt32(iS.ID) == 0)//Encuentra el primero que este vacio
                                            {
                                                iS.ID = Convert.ToInt32(r.Informacion[i]);
                                                iS.DirBloque = (long)file.Length;
                                                iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                {
                                                    if (iss.DirInformacion == -1)
                                                    {
                                                        iss.DirInformacion = r.DirRegistro;
                                                    }
                                                    break;
                                                }
                                                Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            }
                                            break;
                                        }
                                    }
                                    else//No esta vacio
                                    {
                                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos].Exists(existe => Convert.ToInt32(existe.ID) == Convert.ToInt32(r.Informacion[i])))
                                        {
                                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                            {
                                                if (Convert.ToInt32(iS.ID) == Convert.ToInt32(r.Informacion[i]))
                                                {
                                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                    {
                                                        if (iss.DirInformacion == -1)
                                                        {
                                                            iss.DirInformacion = r.DirRegistro;
                                                            break;
                                                        }
                                                    }
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                            {
                                                if (Convert.ToInt32(iS.ID) == 0)//Encuentra el primero que este vacio
                                                {
                                                    iS.ID = Convert.ToInt32(r.Informacion[i]);
                                                    iS.DirBloque = (long)file.Length;
                                                    iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                    {
                                                        if (iss.DirInformacion == -1)
                                                        {
                                                            iss.DirInformacion = r.DirRegistro;
                                                            break;
                                                        }
                                                    }
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    break;
                                                }
                                            }
                                            DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos] = Archivo.OrdenaIndiceSecundario(DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos], DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                        }
                                    }
                                    break;
                                case 'C':
                                    foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                    {
                                        if (Convert.ToString(iS.ID) != Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))
                                        {
                                            vacio = false;
                                            break;
                                        }
                                    }
                                    if (vacio)//Esta vacio el bloque de indice secundario, entra el primero
                                    {
                                        foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                        {
                                            if (Convert.ToString(iS.ID) == Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))//Encuentra el primero que este vacio
                                            {
                                                iS.ID = Convert.ToString(r.Informacion[i]);
                                                iS.DirBloque = (long)file.Length;
                                                iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                {
                                                    if (iss.DirInformacion == -1)
                                                    {
                                                        iss.DirInformacion = r.DirRegistro;
                                                    }
                                                    break;
                                                }
                                                Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            }
                                            break;
                                        }
                                    }
                                    else//No esta vacio
                                    {
                                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos].Exists(existe => Convert.ToString(existe.ID) == Convert.ToString(r.Informacion[i])))
                                        {
                                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                            {
                                                if (Convert.ToString(iS.ID) == Convert.ToString(r.Informacion[i]))
                                                {
                                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                    {
                                                        if (iss.DirInformacion == -1)
                                                        {
                                                            iss.DirInformacion = r.DirRegistro;
                                                            break;
                                                        }
                                                    }
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                            {
                                                if (Convert.ToString(iS.ID) == Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))//Encuentra el primero que este vacio
                                                {
                                                    iS.ID = Convert.ToString(r.Informacion[i]);
                                                    iS.DirBloque = (long)file.Length;
                                                    iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                    {
                                                        if (iss.DirInformacion == -1)
                                                        {
                                                            iss.DirInformacion = r.DirRegistro;
                                                            break;
                                                        }
                                                    }
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    break;
                                                }
                                            }
                                            DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos] = Archivo.OrdenaIndiceSecundario(DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos], DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                        }
                                    }
                                    break;

                            }
                            pos++;
                            break;
                        #endregion
                        case 5:
                            #region Hash Estatico
                            switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                            {
                                case 'C':
                                    float casilla = Convert.ToSingle(Archivo.stringAASCIIValor(Convert.ToString(r.Informacion[i]))) / Convert.ToSingle(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE.Count);
                                    int cajon = Convert.ToString(casilla)[Convert.ToString(casilla).IndexOf(".") + 1] - 48;

                                    if ((Convert.ToSingle(Archivo.stringAASCIIValor(Convert.ToString(r.Informacion[i]))) % Convert.ToSingle(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE.Count)) == 0)
                                    {
                                        cajon = 0;
                                    }
                                    foreach (HashEstatico_SubBloque hesb in DD.Entidades[posE].HE[cajon].Sub_Bloque)
                                    {
                                        if (Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1).Equals(hesb.Informacion))
                                        {
                                            hesb.Informacion = Convert.ToString(r.Informacion[i]);
                                            hesb.DirInformacion = r.DirRegistro;
                                            break;
                                            //CHECAR SI HAY DESBORDAMIENTO
                                        }
                                    }
                                    Archivo.EscribeHashEstaticoSubBloque(DD.Entidades[pos].HE[cajon].DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], DD.Entidades[pos].HE[cajon].Desbordamiento, DD.Entidades[pos].HE[cajon].Sub_Bloque, rutaReg);
                                    break;
                            }
                            break;
                            #endregion
                        case 6:
                            #region Hash Dinamico
                            if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                            {
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS = Ubica(r.Informacion[i], r.DirRegistro, i, file, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                Archivo.EscribeHashDinamicoBloque(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].DireccionIndice, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS, rutaReg);
                            }
                            break;
                            #endregion
                    }
                }
                comboPK.SelectedIndex = -1;
                comboSub_BloquePK.SelectedIndex = -1;
                comboFK.SelectedIndex = -1;
                comboAtributosFK.SelectedIndex = -1;
                comboSub_BloqueFK.SelectedIndex = -1;
                dtGrid_PK.Rows.Clear();
                dtGrid_Sub_BloquesPK.Rows.Clear();
                dtGrid_FK.Rows.Clear();
                dtGrid_Sub_BloquesFK.Rows.Clear();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private HashDinamico Ubica(object r, long r1, int i, FileInfo file, Atributo at, string rutaReg)
        {
            bool vacio1 = false;
            string valor = Archivo.DecimalaBinariDigitos(Convert.ToInt32(r));
            string v1 = "";/////////////////////////////////////////////////////////////////////////////////////////////
            string v2 = valor.Substring(0, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID);
            int tama = Convert.ToInt32(Math.Pow(2, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID));
            int posSub = -1;
            long tam = 0;
            List<HashDinamico_SubBloqueInformacion> listAux = null;

            for (int k = 0; k < tama; k++)
            {
                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID == 0)
                    v1 = "";
                else
                    v1 = DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].CodigoB.Substring(6 - DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID);
                if (v1.Equals(v2))
                {
                    for (int m = 0; m < DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque.HashInformacion.Count; m++)
                    {
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque.HashInformacion[m].Valor == 0)
                        {
                            posSub = m; //Aqui esta la ubicacion de cual se reordena
                            vacio1 = true; //Si hay lugar
                            break;
                        }
                    }
                    if (!vacio1)/////////////////////////////////////////////////////////////////////////////////////////////
                    {
                        //Aqui se parte de incrementa id
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID < 6)
                        {
                            DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID++;/////////////////////////////////////////////////////////////////////////////////////////////
                            tama = Convert.ToInt32(Math.Pow(2, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID));
                            listAux = new List<HashDinamico_SubBloqueInformacion>();
                            for (int g = 0; g < tama; g++)
                            {
                                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque != null)
                                {
                                    for (int t = 0; t < DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque.HashInformacion.Count; t++)
                                    {
                                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque.HashInformacion[t].Valor != 0)
                                        {
                                            HashDinamico_SubBloqueInformacion aux = DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque.HashInformacion[t];
                                            listAux.Add(aux);
                                        }
                                    }
                                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque = null;
                                }
                            }/////////////////////////////////////////////////////////////////////////////////////////////
                            for (int g = 0; g < tama; g++)
                            {
                                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque == null)
                                {
                                    if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].DirSub_Bloque == -1)
                                    {
                                        file = new FileInfo(rutaReg);
                                        tam = file.Length;
                                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque = Archivo.InicializaHashDinamicoSubBloque(tam, rutaReg);
                                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].DirSub_Bloque = tam;
                                    }
                                    else
                                    {
                                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque = Archivo.InicializaHashDinamicoSubBloque(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].DirSub_Bloque, rutaReg);
                                    }
                                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[g].Sub_Bloque.ID = DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID;
                                }
                            }
                            for (int g = 0; g < 2; g++)
                            {
                                if (g == 0)
                                {
                                    for (int h = 0; h < listAux.Count; h++)
                                    {
                                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS = Ubica(listAux[h].Valor, listAux[h].DirRegistro, i, file, at, rutaReg);/////////////////////////////////////////////////////////////////////////////////////////////
                                    }
                                }
                                else
                                {
                                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS = Ubica(r, r1, i, file, at, rutaReg);
                                }
                            }
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID == 6)/////////////////////////////////////////////////////////////////////////////////////////////
                        {
                            HashDinamico_SubBloque aux = DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque;

                            if (aux.DirDesbordamiento == -1)
                            {
                                file = new FileInfo(rutaReg);
                                tam = file.Length;
                                aux.ListaDesbordamiento = Archivo.InicializaHashDinamicoSubBloque(tam, rutaReg);
                                aux.DirDesbordamiento = tam;
                                aux.ListaDesbordamiento.ID = DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID;

                                for (int m = 0; m < aux.ListaDesbordamiento.HashInformacion.Count; m++)
                                {
                                    if (aux.ListaDesbordamiento.HashInformacion[m].Valor == 0)
                                    {
                                        aux.ListaDesbordamiento.HashInformacion[m].Valor = Convert.ToInt32(r);
                                        aux.ListaDesbordamiento.HashInformacion[m].DirRegistro = r1;
                                        Archivo.EscribeHashDinamicoSubBloque(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].DirSub_Bloque, aux.ListaDesbordamiento, rutaReg);
                                        break;
                                    }
                                }
                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque = aux;

                            }
                            else
                            {
                                HashDinamico_SubBloque aux2 = aux.ListaDesbordamiento; //Comenzamos desde el primer desborde
                                vacio1 = false;
                                posSub = -1;
                                bool salir = true;

                                do
                                {
                                    if (aux2.DirDesbordamiento == -1)/////////////////////////////////////////////////////////////////////////////////////////////
                                    {
                                        for (int m = 0; m < aux2.HashInformacion.Count; m++)
                                        {
                                            if (aux2.HashInformacion[m].Valor == 0)
                                            {
                                                posSub = m; //Aqui esta la ubicacion de cual se reordena
                                                vacio1 = true; //Si hay lugar
                                                break;
                                            }
                                        }
                                        if (!vacio1)/////////////////////////////////////////////////////////////////////////////////////////////
                                        {
                                            file = new FileInfo(rutaReg);
                                            tam = file.Length;
                                            aux2.DirDesbordamiento = tam;
                                            aux2.ListaDesbordamiento = Archivo.InicializaHashDinamicoSubBloque(tam, rutaReg);
                                            aux2.ListaDesbordamiento.ID = DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.ID;
                                            aux2 = aux2.ListaDesbordamiento;
                                            Archivo.EscribeHashDinamicoSubBloque(aux.ListaDesbordamiento.HashInformacion[0].DirSub_Bloque - 4, aux.ListaDesbordamiento, rutaReg);
                                        }
                                        else
                                        {
                                            aux2.HashInformacion[posSub].Valor = Convert.ToInt32(r);
                                            aux2.HashInformacion[posSub].DirRegistro = r1;
                                            long direccion = aux2.HashInformacion[0].DirSub_Bloque - 4;
                                            Archivo.EscribeHashDinamicoSubBloque(direccion, aux2, rutaReg);
                                            salir = false;/////////////////////////////////////////////////////////////////////////////////////////////
                                        }
                                    }
                                    else
                                        aux2 = aux2.ListaDesbordamiento;
                                } while (salir);

                            }
                        }
                    }
                    else
                    {
                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque.HashInformacion[posSub].Valor = Convert.ToInt32(r);/////////////////////////////////////////////////////////////////////////////////////////////
                        DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque.HashInformacion[posSub].DirRegistro = r1;

                    }
                }
                Archivo.EscribeHashDinamicoSubBloque(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].DirSub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS.BloquePrincipal[k].Sub_Bloque, rutaReg);
            }

            return DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS;
        }

        private void button_Elimina_Registro_Click(object sender, EventArgs e)
        {
            Registro regAux;
            char cAux;
            int iAux;
            string sAux;
            string rutaReg = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".dat";
            int pos = 0;

            regAux = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex];
            DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.RemoveAt(dtGrid_Registros.CurrentCell.RowIndex);
            DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = Archivo.ActualizaRegistros(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros, DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
            if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count > 0)
            {
                DD.Entidades[comboEntidadesRegistros.SelectedIndex].DireccionDato = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[0].DirRegistro;//Solo es si hay mas de dos
            }
            else
            {
                DD.Entidades[comboEntidadesRegistros.SelectedIndex].DireccionDato = -1;
            }
            Archivo.EscribeEntidad(DD.Entidades[comboEntidadesRegistros.SelectedIndex], nomArchivo);
            dtGrid_Entidades.Rows.Clear();
            foreach (Entidad en in DD.Entidades)
            {
                dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
            }
            dtGrid_Registros.Rows.Clear();
            foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
            {
                dtGrid_Registros.Rows.Add();
            }
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
            {
                dtGrid_Registros[0, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirRegistro;
                dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirSigRegistro;
            }
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
            {
                for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                {
                    dtGrid_Registros[j, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].Informacion[j - 1];
                }
            }
            //Van los indices
            //regAux
            pos = 0;
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
            {
                switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice)
                {
                    case 2:
                        #region Indice Primario
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            sAux = Convert.ToString(regAux.Informacion[i]);
                            iAux = Convert.ToInt32(sAux[0]) - 48;
                            foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                            {
                                if (iAux == Convert.ToChar(ip.ID))
                                {
                                    foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                    {
                                        if (regAux.DirRegistro == ips.DirInformacion)
                                        {
                                            ips.Informacion = 0;
                                            ips.DirInformacion = -1;
                                        }
                                        ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                        Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                    }
                                }
                            }
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                        {
                            sAux = Convert.ToString(regAux.Informacion[i]);
                            cAux = Convert.ToChar(sAux[0]);
                            foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                            {
                                if (cAux == Convert.ToChar(ip.ID))
                                {
                                    foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                    {
                                        if (regAux.DirRegistro == ips.DirInformacion)
                                        {
                                            ips.Informacion = Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1);
                                            ips.DirInformacion = -1;
                                        }
                                        ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                        Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    case 3:
                        #region Indice Secundario
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                            {
                                if (Convert.ToInt32(iS.ID) == Convert.ToInt32(regAux.Informacion[i]))
                                {
                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                    {
                                        if (iss.DirInformacion == regAux.DirRegistro)
                                        {
                                            iss.DirInformacion = -1;
                                            break;
                                        }
                                    }
                                    iS.Sub_Bloque = Archivo.OrdenaSubIndicesSecundarios(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                }
                            }
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                        {
                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                            {
                                if (Convert.ToString(iS.ID) == Convert.ToString(regAux.Informacion[i]))
                                {
                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                    {
                                        if (iss.DirInformacion == regAux.DirRegistro)
                                        {
                                            iss.DirInformacion = -1;
                                            break;
                                        }
                                    }
                                    iS.Sub_Bloque = Archivo.OrdenaSubIndicesSecundarios(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                }
                            }
                        }
                        pos++;
                        break;
                    #endregion
                    case 5:
                        #region Hash Estatico
                        switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                        {
                            case 'C':
                                foreach (HashEstatico heb in DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE)
                                {
                                    foreach (HashEstatico_SubBloque h in heb.Sub_Bloque)
                                    {
                                        if (h.DirInformacion == regAux.DirRegistro)
                                        {
                                            h.Informacion = Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1);
                                            h.DirInformacion = -1;
                                            Archivo.OrdenaHashHashEstaticoSubBloque(heb.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                            Archivo.EscribeHashEstaticoSubBloque(heb.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], heb.Desbordamiento, heb.Sub_Bloque, nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            break;
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                        #endregion
                }
            }
            comboPK.SelectedIndex = -1;
            comboSub_BloquePK.SelectedIndex = -1;
            comboFK.SelectedIndex = -1;
            comboAtributosFK.SelectedIndex = -1;
            comboSub_BloqueFK.SelectedIndex = -1;
            dtGrid_PK.Rows.Clear();
            dtGrid_Sub_BloquesPK.Rows.Clear();
            dtGrid_FK.Rows.Clear();
            dtGrid_Sub_BloquesFK.Rows.Clear();
        }

        private void button_Modifica_Registro_Click(object sender, EventArgs e)
        {
            string rutaReg = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".dat";
            string cadAnt = "";
            object infoAux;
            int iAux;
            char cAux;
            string sAux;
            int posE = comboEntidadesRegistros.SelectedIndex;
            long tam;
            int pos = 0;
            bool vacio = true;
            FileInfo file = new FileInfo(nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
            Registro r = new Registro();
            Registro regAux = new Registro();

            regAux = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex];
            pos = 0;
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
            {
                switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice)
                {
                    case 2:
                        #region Indice Primario
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            sAux = Convert.ToString(regAux.Informacion[i]);
                            iAux = Convert.ToInt32(sAux[0]) - 48;
                            foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                            {
                                if (iAux == Convert.ToChar(ip.ID))
                                {
                                    foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                    {
                                        if (regAux.DirRegistro == ips.DirInformacion)
                                        {
                                            ips.Informacion = 0;
                                            ips.DirInformacion = -1;
                                        }
                                        ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                        Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                    }
                                }
                            }
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                        {
                            sAux = Convert.ToString(regAux.Informacion[i]);
                            cAux = Convert.ToChar(sAux[0]);
                            foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                            {
                                if (cAux == Convert.ToChar(ip.ID))
                                {
                                    foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                    {
                                        if (regAux.DirRegistro == ips.DirInformacion)
                                        {
                                            ips.Informacion = Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1);
                                            ips.DirInformacion = -1;
                                        }
                                        ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                        Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    case 3:
                        #region Indice Secundario
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                            {
                                if (Convert.ToInt32(iS.ID) == Convert.ToInt32(regAux.Informacion[i]))
                                {
                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                    {
                                        if (iss.DirInformacion == regAux.DirRegistro)
                                        {
                                            iss.DirInformacion = -1;
                                            break;
                                        }
                                    }
                                    iS.Sub_Bloque = Archivo.OrdenaSubIndicesSecundarios(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                }
                            }
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                        {
                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                            {
                                if (Convert.ToString(iS.ID) == Convert.ToString(regAux.Informacion[i]))
                                {
                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                    {
                                        if (iss.DirInformacion == regAux.DirRegistro)
                                        {
                                            iss.DirInformacion = -1;
                                            break;
                                        }
                                    }
                                    iS.Sub_Bloque = Archivo.OrdenaSubIndicesSecundarios(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                }
                            }
                        }
                        pos++;
                        break;
                    #endregion
                    case 5:
                        #region Hash Estatico
                        switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                        {
                            case 'C':
                                foreach (HashEstatico heb in DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE)
                                {
                                    foreach (HashEstatico_SubBloque h in heb.Sub_Bloque)
                                    {
                                        if (h.DirInformacion == regAux.DirRegistro)
                                        {
                                            h.Informacion = Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1);
                                            h.DirInformacion = -1;
                                            Archivo.OrdenaHashHashEstaticoSubBloque(heb.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                            Archivo.EscribeHashEstaticoSubBloque(heb.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], heb.Desbordamiento, heb.Sub_Bloque, nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            break;
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                        #endregion
                }
            }
            #region Registros
            //Aqui guardo el valor anterior del atributo
            if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[dtGrid_Registros.CurrentCell.ColumnIndex - 1].TipoDato == 'E')
            {
                //intAnt = (int)DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex].Informacion[dtGrid_Registros.CurrentCell.ColumnIndex - 1];
            }
            else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[dtGrid_Registros.CurrentCell.ColumnIndex - 1].TipoDato == 'C')
            {
                cadAnt = (string)DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex].Informacion[dtGrid_Registros.CurrentCell.ColumnIndex - 1];
            }

            if (!dtGrid_Registros.CurrentCell.Value.Equals(""))
            {
                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[dtGrid_Registros.CurrentCell.ColumnIndex - 1].TipoDato == 'E')
                {
                    infoAux = dtGrid_Registros.CurrentCell.Value;
                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex].Informacion[dtGrid_Registros.CurrentCell.ColumnIndex - 1] = Archivo.RellenaNombres((string)dtGrid_Registros.CurrentCell.Value, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[dtGrid_Registros.CurrentCell.ColumnIndex - 1].Longitud - 1);
                }
                else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[dtGrid_Registros.CurrentCell.ColumnIndex - 1].TipoDato == 'C')
                {
                    infoAux = dtGrid_Registros.CurrentCell.Value;
                    cadAnt = (string)DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex].Informacion[dtGrid_Registros.CurrentCell.ColumnIndex - 1];
                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex].Informacion[dtGrid_Registros.CurrentCell.ColumnIndex - 1] = Archivo.RellenaNombres((string)dtGrid_Registros.CurrentCell.Value, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[dtGrid_Registros.CurrentCell.ColumnIndex - 1].Longitud - 1);
                }
            }
            Archivo.EscribeRegistro(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex], DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
            //Este es el nuevo
            r = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[dtGrid_Registros.CurrentCell.RowIndex];

            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
            {
                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice == 1)
                {
                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.OrderBy(reg => reg.Informacion[i]).ToList();
                }
            }
            DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = Archivo.ActualizaRegistros(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros, DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
            DD.Entidades[comboEntidadesRegistros.SelectedIndex].DireccionDato = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[0].DirRegistro;
            Archivo.EscribeEntidad(DD.Entidades[comboEntidadesRegistros.SelectedIndex], nomArchivo);
            dtGrid_Entidades.Rows.Clear();

            foreach (Entidad en in DD.Entidades)
            {
                dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
            }
            dtGrid_Registros.Rows.Clear();
            foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
            {
                dtGrid_Registros.Rows.Add();
            }
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
            {
                dtGrid_Registros[0, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirRegistro;
                dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirSigRegistro;
            }
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
            {
                for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                {
                    dtGrid_Registros[j, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].Informacion[j - 1];
                }
            }
            #endregion
            //RECUARDA QUE APARTIR DE AQUI RUTA DE ARCHIVO IDX SE PONE DIRECTO
            pos = 0;
            for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
            {
                //Indice Primario
                switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice)
                {
                    case 2:
                        #region Indice Primario
                        switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                        {
                            case 'E':
                                sAux = Convert.ToString(r.Informacion[i]);
                                iAux = Convert.ToInt32(sAux[0]) - 48;
                                foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                                {
                                    if (iAux == Convert.ToChar(ip.ID))
                                    {
                                        if (ip.DirBloque == -1)
                                        {//No hay ninguno, osea que será el primero
                                            tam = (long)file.Length;
                                            ip.DirBloque = tam;
                                            ip.Sub_Bloque = Archivo.InicializaSub_BloqueIndicePrimario(ip.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                            {
                                                if (Convert.ToInt32(ips.Informacion) == 0)
                                                {
                                                    ips.Informacion = Convert.ToInt32(r.Informacion[i]);
                                                    ips.DirInformacion = r.DirRegistro;
                                                    break;
                                                }
                                            }
                                            Archivo.EscribeIndicePrimario(ip, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        }
                                        else
                                        {//Ya no es el primero
                                            foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                            {
                                                if (Convert.ToInt32(ips.Informacion) == 0)
                                                {
                                                    ips.Informacion = Convert.ToInt32(r.Informacion[i]);
                                                    ips.DirInformacion = r.DirRegistro;
                                                    break;
                                                }
                                            }
                                            ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                            Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        }
                                    }
                                }
                                break;
                            case 'C':
                                sAux = Convert.ToString(r.Informacion[i]);
                                cAux = Convert.ToChar(sAux[0]);
                                foreach (IndicePrimario ip in DD.Entidades[comboEntidadesRegistros.SelectedIndex].PKs)
                                {
                                    if (cAux == Convert.ToChar(ip.ID))
                                    {
                                        if (ip.DirBloque == -1)
                                        {//No hay ninguno, osea que será el primero
                                            tam = (long)file.Length;
                                            ip.DirBloque = tam;
                                            ip.Sub_Bloque = Archivo.InicializaSub_BloqueIndicePrimario(ip.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                            {
                                                if (Convert.ToString(ips.Informacion).Equals(Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1)))
                                                {
                                                    ips.Informacion = Convert.ToString(r.Informacion[i]);
                                                    ips.DirInformacion = r.DirRegistro;
                                                    break;
                                                }
                                            }
                                            Archivo.EscribeIndicePrimario(ip, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        }
                                        else
                                        {//Ya no es el primero
                                            foreach (IndicePrimario_SubBloque ips in ip.Sub_Bloque)
                                            {
                                                if (Convert.ToString(ips.Informacion).Equals(Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1)))
                                                {
                                                    ips.Informacion = Convert.ToString(r.Informacion[i]);
                                                    ips.DirInformacion = r.DirRegistro;
                                                    break;
                                                }
                                            }
                                            ip.Sub_Bloque = Archivo.OrdenaSubIndicesPrimarios(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i]);
                                            Archivo.EscribeSub_BloqueIndicePrimario(ip.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");

                                        }
                                    }
                                }
                                comboPK.Text = "";
                                comboSub_BloquePK.Text = "";
                                dtGrid_PK.Rows.Clear();
                                dtGrid_Sub_BloquesPK.Rows.Clear();
                                break;
                        }
                        break;
                    #endregion
                    case 3:
                        #region Indice Secundario
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                            {
                                if (Convert.ToInt32(iS.ID) != 0)
                                {
                                    vacio = false;
                                    break;
                                }
                            }
                            if (vacio)//Esta vacio el bloque de indice secundario, entra el primero
                            {
                                foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                {
                                    if (Convert.ToInt32(iS.ID) == 0)//Encuentra el primero que este vacio
                                    {
                                        iS.ID = Convert.ToInt32(r.Informacion[i]);
                                        iS.DirBloque = (long)file.Length;
                                        iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                        {
                                            if (iss.DirInformacion == -1)
                                            {
                                                iss.DirInformacion = r.DirRegistro;
                                                Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            }
                                            break;
                                        }
                                        Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                    }
                                    break;
                                }
                            }
                            else//No esta vacio
                            {
                                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos].Exists(existe => Convert.ToInt32(existe.ID) == Convert.ToInt32(r.Informacion[i])))
                                {
                                    foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                    {
                                        if (Convert.ToInt32(iS.ID) == Convert.ToInt32(r.Informacion[i]))
                                        {
                                            foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                            {
                                                if (iss.DirInformacion == -1)
                                                {
                                                    iss.DirInformacion = r.DirRegistro;
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                                    break;
                                                }
                                            }
                                            Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                    {
                                        if (Convert.ToInt32(iS.ID) == 0)//Encuentra el primero que este vacio
                                        {
                                            iS.ID = Convert.ToInt32(r.Informacion[i]);
                                            iS.DirBloque = (long)file.Length;
                                            iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                            {
                                                if (iss.DirInformacion == -1)
                                                {
                                                    iss.DirInformacion = r.DirRegistro;
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                                    break;
                                                }
                                            }

                                            Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            break;
                                        }
                                    }
                                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos] = Archivo.OrdenaIndiceSecundario(DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos], DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                }
                            }
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                        {
                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                            {
                                if (Convert.ToString(iS.ID) != Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))
                                {
                                    vacio = false;
                                    break;
                                }
                            }
                            if (vacio)//Esta vacio el bloque de indice secundario, entra el primero
                            {
                                foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                {
                                    if (Convert.ToString(iS.ID) == Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))//Encuentra el primero que este vacio
                                    {
                                        iS.ID = Convert.ToString(r.Informacion[i]);
                                        iS.DirBloque = (long)file.Length;
                                        iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                        {
                                            if (iss.DirInformacion == -1)
                                            {
                                                iss.DirInformacion = r.DirRegistro;
                                                Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            }
                                            break;
                                        }
                                        Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                    }
                                    break;
                                }
                            }
                            else//No esta vacio
                            {
                                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos].Exists(existe => Convert.ToString(existe.ID) == Convert.ToString(r.Informacion[i])))
                                {
                                    foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                    {
                                        if (Convert.ToString(iS.ID) == Convert.ToString(r.Informacion[i]))
                                        {
                                            foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                            {
                                                if (iss.DirInformacion == -1)
                                                {
                                                    iss.DirInformacion = r.DirRegistro;
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                                    break;
                                                }
                                            }
                                            Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                    {
                                        if (Convert.ToString(iS.ID) == Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))//Encuentra el primero que este vacio
                                        {
                                            iS.ID = Convert.ToString(r.Informacion[i]);
                                            iS.DirBloque = (long)file.Length;
                                            iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                            {
                                                if (iss.DirInformacion == -1)
                                                {
                                                    iss.DirInformacion = r.DirRegistro;
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                                    break;
                                                }
                                            }
                                            Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                            break;
                                        }
                                    }
                                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos] = Archivo.OrdenaIndiceSecundario(DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos], DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                }
                            }
                        }
                        pos++;
                        break;
                    #endregion
                    case 5:
                        #region Hash Estatico
                        switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                        {
                            case 'C':
                                float casilla = Convert.ToSingle(Archivo.stringAASCIIValor(Convert.ToString(r.Informacion[i]))) / Convert.ToSingle(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE.Count);
                                int cajon = Convert.ToString(casilla)[Convert.ToString(casilla).IndexOf(".") + 1] - 48;

                                if ((Convert.ToSingle(Archivo.stringAASCIIValor(Convert.ToString(r.Informacion[i]))) % Convert.ToSingle(DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE.Count)) == 0)
                                {
                                    cajon = 0;
                                }
                                foreach (HashEstatico_SubBloque hesb in DD.Entidades[comboEntidadesRegistros.SelectedIndex].HE[cajon].Sub_Bloque)
                                {
                                    if (Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1).Equals(hesb.Informacion))
                                    {
                                        hesb.Informacion = Convert.ToString(r.Informacion[i]);
                                        hesb.DirInformacion = r.DirRegistro;
                                        break;
                                        //CHECAR SI HAY DESBORDAMIENTO
                                    }
                                }
                                Archivo.EscribeHashEstaticoSubBloque(DD.Entidades[posE].HE[cajon].DirBloque, DD.Entidades[posE].Atributos[i], -1, DD.Entidades[posE].HE[cajon].Sub_Bloque, nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx");
                                break;
                        }
                        break;
                        #endregion

                }
            }
            comboPK.SelectedIndex = -1;
            comboSub_BloquePK.SelectedIndex = -1;
            comboFK.SelectedIndex = -1;
            comboAtributosFK.SelectedIndex = -1;
            comboSub_BloqueFK.SelectedIndex = -1;
            dtGrid_PK.Rows.Clear();
            dtGrid_Sub_BloquesPK.Rows.Clear();
            dtGrid_FK.Rows.Clear();
            dtGrid_Sub_BloquesFK.Rows.Clear();
        }


        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DD = null;
                TabControl1.Enabled = false;
                txtB_Cabecera.Text = "-1";
                txtB_Longitud.Text = "";
                txtB_NomAtributo.Text = "";
                txtB_NomEntidad.Text = "";
                dtGrid_Atributos.Rows.Clear();
                dtGrid_Entidades.Rows.Clear();
                dtGrid_PK.Rows.Clear();
                dtGrid_Registros.Columns.Clear();
                dtGrid_Registros.Rows.Clear();
                comboEntidades.Items.Clear();
                comboPK.Items.Clear();
                comboEntidadesRegistros.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en cerrar el diccionario de datos(DD):\n" + ex.Message);
            }
        }

        private void comboPK_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = comboPK.SelectedIndex;
            nomRegistro = Path.GetDirectoryName(nomArchivo);
            FileInfo f = null;

            if (pos != -1)
            {
                f = new FileInfo(nomRegistro + @"\" + DD.Entidades[pos].Nombre + ".idx");

                dtGrid_PK.Rows.Clear();
                comboSub_BloquePK.Items.Clear();
                if (f.Exists)
                {
                    foreach (Atributo at in DD.Entidades[pos].Atributos)
                    {
                        if (at.TipoIndice == 2)
                        {
                            foreach (IndicePrimario pk in DD.Entidades[pos].PKs)
                            {
                                dtGrid_PK.Rows.Add(pk.ID, pk.DirBloque);
                                comboSub_BloquePK.Items.Add(pk.ID);
                                comboSub_BloquePK.Text = "";
                            }
                        }
                    }
                }
                else
                {
                    dtGrid_PK.Rows.Clear();
                }
            }
        }

        private void comboFK_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = comboFK.SelectedIndex;

            comboAtributosFK.Items.Clear();
            if (pos != -1)
            {
                foreach (Atributo at in DD.Entidades[pos].Atributos)
                {
                    if (at.TipoIndice == 3)
                    {
                        comboAtributosFK.Items.Add(at.Nombre);
                    }
                }
            }
        }

        private void comboAtributosFK_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos1 = comboFK.SelectedIndex;
            int pos2 = comboAtributosFK.SelectedIndex;
            nomRegistro = Path.GetDirectoryName(nomArchivo);
            FileInfo f = null;

            if (pos1 != -1 && pos2 != -1)
            {
                f = new FileInfo(nomRegistro + @"\" + DD.Entidades[pos1].Nombre + ".idx");

                comboSub_BloqueFK.Items.Clear();
                if (f.Exists)
                {
                    dtGrid_FK.Rows.Clear();
                    foreach (IndiceSecundario iS in DD.Entidades[pos1].FKs[pos2])
                    {
                        if (iS.DirBloque != -1)
                        {
                            dtGrid_FK.Rows.Add(iS.ID, iS.DirBloque);
                            comboSub_BloqueFK.Items.Add(iS.ID);
                        }

                    }
                }
                else
                {
                    dtGrid_FK.Rows.Clear();
                    comboFK.Items.Clear();
                    comboAtributosFK.Items.Clear();
                    comboSub_BloqueFK.Items.Clear();
                }

            }
        }

        private void comboSub_BloqueFK_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos1 = comboFK.SelectedIndex;
            int pos2 = comboAtributosFK.SelectedIndex;
            int pos3 = comboSub_BloqueFK.SelectedIndex;
            nomRegistro = Path.GetDirectoryName(nomArchivo);
            FileInfo f = null;

            if (pos1 != -1 && pos2 != -1 && pos3 != -1)
            {
                f = new FileInfo(nomRegistro + @"\" + DD.Entidades[pos1].Nombre + ".idx");

                if (f.Exists)
                {
                    dtGrid_Sub_BloquesFK.Rows.Clear();
                    foreach (IndiceSecundario_SubBloque iss in DD.Entidades[pos1].FKs[pos2][pos3].Sub_Bloque)
                    {
                        dtGrid_Sub_BloquesFK.Rows.Add(iss.DirInformacion);
                    }
                }
                else
                {
                    dtGrid_FK.Rows.Clear();
                    comboFK.Items.Clear();
                    comboAtributosFK.Items.Clear();
                    comboSub_BloqueFK.Items.Clear();
                }
            }

        }

        private void button_Importacion_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            string nameFile;
            string read = "";
            List<string> renglones = new List<string>();
            List<object> informacioLeida = new List<object>(1);
            double cont;

            if (open.ShowDialog() == DialogResult.OK)
            {
                StreamReader archivo;
                nameFile = open.FileName;
                archivo = new StreamReader(nameFile);

                labelProgress.Visible = true;
                while (read != null)
                {
                    read = archivo.ReadLine();
                    if (read != null && read != "")
                    {
                        renglones.Add(read);
                    }
                }
                foreach (string s in renglones)
                {
                    object[] auxInfo = null;

                    auxInfo = s.Split(',');
                    informacioLeida.Add(auxInfo);
                }
                cont = Convert.ToDouble(100 / informacioLeida.Count);
                foreach (object[] info in informacioLeida)
                {
                    #region Declaracion de variables locales
                    Registro r = new Registro();
                    string rutaReg = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".dat";
                    FileInfo file = new FileInfo(rutaReg);
                    char cAux;
                    int iAux;
                    string sAux;
                    long tam;
                    bool vacio = true;
                    int pos = 0;
                    int valor;
                    #endregion

                    #region Registros
                    r.DirRegistro = (long)file.Length;
                    for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
                    {
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            valor = Convert.ToInt32(info[i]);
                            r.Informacion.Add(Archivo.RellenaNombres(Convert.ToString(valor), DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1));
                        }
                        else
                        {
                            r.Informacion.Add(Archivo.RellenaNombres(Convert.ToString(info[i]), DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1));
                        }
                    }
                    r.DirSigRegistro = -1;
                    Archivo.EscribeRegistro(r, DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Add(r);
                    for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
                    {
                        if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice == 1 && DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'C')
                        {
                            DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.OrderBy(reg => reg.Informacion[i]).ToList();
                        }
                        else if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice == 1 && DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                        {
                            DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.OrderBy(reg => reg.Informacion[i]).ToList();
                        }
                    }
                    labelProgress.Text = Convert.ToString(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count);
                    Console.WriteLine(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count);
                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros = Archivo.ActualizaRegistros(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros, DD.Entidades[comboEntidadesRegistros.SelectedIndex], rutaReg);
                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].DireccionDato = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[0].DirRegistro;
                    Archivo.EscribeEntidad(DD.Entidades[comboEntidadesRegistros.SelectedIndex], nomArchivo);
                    #endregion
                    #region Indices
                    rutaReg = nomRegistro + @"\" + DD.Entidades[comboEntidadesRegistros.SelectedIndex].Nombre + ".idx";
                    file = new FileInfo(rutaReg);
                    pos = 0;
                    for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count; i++)
                    {
                        //Indice Primario
                        switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoIndice)
                        {
                            case 2:
                                #region Indice Primario

                                break;
                            #endregion
                            case 3:
                                #region Indice Secundario
                                file = new FileInfo(rutaReg);
                                switch (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato)
                                {
                                    case 'E':
                                        foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                        {
                                            if (Convert.ToInt32(iS.ID) != 0)
                                            {
                                                vacio = false;
                                                break;
                                            }
                                        }
                                        if (vacio)//Esta vacio el bloque de indice secundario, entra el primero
                                        {
                                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                            {
                                                if (Convert.ToInt32(iS.ID) == 0)//Encuentra el primero que este vacio
                                                {
                                                    iS.ID = Convert.ToInt32(r.Informacion[i]);
                                                    iS.DirBloque = (long)file.Length;
                                                    iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                    {
                                                        if (iss.DirInformacion == -1)
                                                        {
                                                            iss.DirInformacion = r.DirRegistro;
                                                        }
                                                        break;
                                                    }
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                }
                                                break;
                                            }
                                        }
                                        else//No esta vacio
                                        {
                                            if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos].Exists(existe => Convert.ToInt32(existe.ID) == Convert.ToInt32(r.Informacion[i])))
                                            {
                                                foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                                {
                                                    if (Convert.ToInt32(iS.ID) == Convert.ToInt32(r.Informacion[i]))
                                                    {
                                                        foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                        {
                                                            if (iss.DirInformacion == -1)
                                                            {
                                                                iss.DirInformacion = r.DirRegistro;
                                                                break;
                                                            }
                                                        }
                                                        Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                                {
                                                    if (Convert.ToInt32(iS.ID) == 0)//Encuentra el primero que este vacio
                                                    {
                                                        iS.ID = Convert.ToInt32(r.Informacion[i]);
                                                        iS.DirBloque = (long)file.Length;
                                                        iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                        foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                        {
                                                            if (iss.DirInformacion == -1)
                                                            {
                                                                iss.DirInformacion = r.DirRegistro;
                                                                break;
                                                            }
                                                        }
                                                        Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                        Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                        break;
                                                    }
                                                }
                                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos] = Archivo.OrdenaIndiceSecundario(DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos], DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            }
                                        }
                                        break;
                                    case 'C':
                                        foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                        {
                                            if (Convert.ToString(iS.ID) != Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))
                                            {
                                                vacio = false;
                                                break;
                                            }
                                        }
                                        if (vacio)//Esta vacio el bloque de indice secundario, entra el primero
                                        {
                                            foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                            {
                                                if (Convert.ToString(iS.ID) == Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))//Encuentra el primero que este vacio
                                                {
                                                    iS.ID = Convert.ToString(r.Informacion[i]);
                                                    iS.DirBloque = (long)file.Length;
                                                    iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                    {
                                                        if (iss.DirInformacion == -1)
                                                        {
                                                            iss.DirInformacion = r.DirRegistro;
                                                        }
                                                        break;
                                                    }
                                                    Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                }
                                                break;
                                            }
                                        }
                                        else//No esta vacio
                                        {
                                            if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos].Exists(existe => Convert.ToString(existe.ID) == Convert.ToString(r.Informacion[i])))
                                            {
                                                foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                                {
                                                    if (Convert.ToString(iS.ID) == Convert.ToString(r.Informacion[i]))
                                                    {
                                                        foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                        {
                                                            if (iss.DirInformacion == -1)
                                                            {
                                                                iss.DirInformacion = r.DirRegistro;
                                                                break;
                                                            }
                                                        }
                                                        Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (IndiceSecundario iS in DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos])
                                                {
                                                    if (Convert.ToString(iS.ID) == Archivo.RellenaNombres("", DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].Longitud - 1))//Encuentra el primero que este vacio
                                                    {
                                                        iS.ID = Convert.ToString(r.Informacion[i]);
                                                        iS.DirBloque = (long)file.Length;
                                                        iS.Sub_Bloque = Archivo.InicializaSub_BloqueIndiceSecundario(iS.DirBloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                        foreach (IndiceSecundario_SubBloque iss in iS.Sub_Bloque)
                                                        {
                                                            if (iss.DirInformacion == -1)
                                                            {
                                                                iss.DirInformacion = r.DirRegistro;
                                                                break;
                                                            }
                                                        }
                                                        Archivo.EscribeSub_BloqueIndiceSecundario(iS.Sub_Bloque, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                        Archivo.EscribeIndiceSecundario(iS, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                                        break;
                                                    }
                                                }
                                                DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos] = Archivo.OrdenaIndiceSecundario(DD.Entidades[comboEntidadesRegistros.SelectedIndex].FKs[pos], DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                            }
                                        }
                                        break;

                                }
                                pos++;
                                break;
                            #endregion
                            case 6:
                                #region Hash Dinamico
                                if (DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].TipoDato == 'E')
                                {
                                    DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS = Ubica(r.Informacion[i], r.DirRegistro, i, file, DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i], rutaReg);
                                    Archivo.EscribeHashDinamicoBloque(DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos[i].DireccionIndice, DD.Entidades[comboEntidadesRegistros.SelectedIndex].HS, rutaReg);

                                }
                                break;
                                #endregion
                        }
                    }

                    #endregion
                }
                dtGrid_Entidades.Rows.Clear();
                foreach (Entidad en in DD.Entidades)
                {
                    dtGrid_Entidades.Rows.Add(en.Nombre, en.DireccionEntidad, en.DireccionAtributo, en.DireccionDato, en.DireccionSigEntidad);
                }
                dtGrid_Registros.Rows.Clear();
                foreach (Registro reg in DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros)
                {
                    dtGrid_Registros.Rows.Add();
                }
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
                {
                    dtGrid_Registros[0, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirRegistro;
                    dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].DirSigRegistro;
                }
                for (int i = 0; i < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros.Count; i++)
                {
                    for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                    {
                        dtGrid_Registros[j, i].Value = DD.Entidades[comboEntidadesRegistros.SelectedIndex].Registros[i].Informacion[j - 1];
                    }
                }
                comboPK.SelectedIndex = -1;
                comboFK.SelectedIndex = -1;
                comboAtributosFK.SelectedIndex = -1;
                comboSub_BloqueFK.SelectedIndex = -1;
                dtGrid_PK.Rows.Clear();
                dtGrid_FK.Rows.Clear();
                dtGrid_Sub_BloquesFK.Rows.Clear();
            }

        }

        private void comboHashDinamico_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = comboHashDinamico.SelectedIndex;
            nomRegistro = Path.GetDirectoryName(nomArchivo);
            FileInfo f = null;

            if (pos != -1)
            {
                f = new FileInfo(nomRegistro + @"\" + DD.Entidades[pos].Nombre + ".idx");

                dtGrid_HashDinamico.Rows.Clear();
                comboBinario.Items.Clear();
                if (f.Exists && DD.Entidades[pos].Atributos.Any(existe => existe.TipoIndice == 6))
                {
                    txtB_IDHashDinamico.Text = Convert.ToString(DD.Entidades[pos].HS.ID);
                    if (DD.Entidades[pos].HS.ID == 0)
                    {
                        comboBinario.Items.Add("0");
                        dtGrid_HashDinamico.Rows.Add(0, DD.Entidades[pos].HS.BloquePrincipal[0].DirSub_Bloque);
                    }
                    else
                    {
                        for (int i = 0; i < Math.Pow(2, DD.Entidades[pos].HS.ID); i++)
                        {
                            string auxS = "";

                            auxS = DD.Entidades[pos].HS.BloquePrincipal[i].CodigoB.Substring(6 - DD.Entidades[pos].HS.ID);
                            comboBinario.Items.Add(auxS);
                            dtGrid_HashDinamico.Rows.Add(auxS, DD.Entidades[pos].HS.BloquePrincipal[i].DirSub_Bloque);
                        }
                    }
                }
                else
                {
                    dtGrid_HashDinamico.Rows.Clear();
                    dtGrid_HashDinamicoSub_Bloque.Rows.Clear();
                    comboBinario.Items.Clear();
                    txtB_IDHashDinamico.Text = "";
                    txtB_IDHashDinamicoSub_Bloque.Text = "";
                }
            }
        }

        private void comboBinario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = comboHashDinamico.SelectedIndex;
            int pos1 = comboBinario.SelectedIndex;
            long dirSig = 0;
            HashDinamico_SubBloque auxlist;
            List<HashDinamico_SubBloqueInformacion> listaOrden = new List<HashDinamico_SubBloqueInformacion>();
            List<HashDinamico_SubBloqueInformacion> listaOrden2 = new List<HashDinamico_SubBloqueInformacion>();

            dtGrid_HashDinamicoSub_Bloque.Rows.Clear();
            if (pos != -1 && pos1 != -1)
            {
                if (DD.Entidades[pos].HS.BloquePrincipal[pos1].DirSub_Bloque != -1)
                {
                    txtB_IDHashDinamicoSub_Bloque.Text = Convert.ToString(DD.Entidades[pos].HS.BloquePrincipal[pos1].Sub_Bloque.ID);
                    foreach (HashDinamico_SubBloqueInformacion hdsbi in DD.Entidades[pos].HS.BloquePrincipal[pos1].Sub_Bloque.HashInformacion)
                    {
                        dtGrid_HashDinamicoSub_Bloque.Rows.Add(hdsbi.Valor, hdsbi.DirRegistro);
                        listaOrden.Add(hdsbi);
                    }
                    dirSig = DD.Entidades[pos].HS.BloquePrincipal[pos1].Sub_Bloque.DirDesbordamiento;
                    auxlist = DD.Entidades[pos].HS.BloquePrincipal[pos1].Sub_Bloque.ListaDesbordamiento;
                    while (dirSig != -1)
                    {
                        foreach (HashDinamico_SubBloqueInformacion hdsbi in auxlist.HashInformacion)
                        {
                            dtGrid_HashDinamicoSub_Bloque.Rows.Add(hdsbi.Valor, hdsbi.DirRegistro);
                            listaOrden.Add(hdsbi);
                        }
                        dirSig = auxlist.DirDesbordamiento;
                        auxlist = auxlist.ListaDesbordamiento;
                    }
                    dtGrid_HashDinamicoSub_Bloque.Rows.Clear();
                    for (int i = 0; i < listaOrden.Count; i++)
                    {
                        if (listaOrden[i].Valor != 0)
                        {
                            listaOrden2.Add(listaOrden[i]);
                        }
                    }
                    listaOrden2 = listaOrden2.OrderBy(ord => ord.Valor).ToList();
                    for (int i = 0; i < listaOrden.Count; i++)
                    {
                        if (i < listaOrden2.Count)
                        {
                            dtGrid_HashDinamicoSub_Bloque.Rows.Add(listaOrden2[i].Valor, listaOrden2[i].DirRegistro);
                        }
                        else
                        {
                            dtGrid_HashDinamicoSub_Bloque.Rows.Add(0, -1);
                        }
                        if (i == 19 || i == 39 || i == 59 || i == 79)
                        {
                            dtGrid_HashDinamicoSub_Bloque.Rows.Add("------------", "------------");
                        }
                    }
                }
                else
                {
                    dtGrid_HashDinamico.Rows.Clear();
                    dtGrid_HashDinamicoSub_Bloque.Rows.Clear();
                    comboBinario.Items.Clear();
                    txtB_IDHashDinamico.Text = "";
                    txtB_IDHashDinamicoSub_Bloque.Text = "";
                }
            }
        }

        private void comboSub_BloquePK_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos1 = comboPK.SelectedIndex;
            int pos2 = comboSub_BloquePK.SelectedIndex;


            if (pos1 != -1 && pos2 != -1)
            {
                dtGrid_Sub_BloquesPK.Rows.Clear();
                if (DD.Entidades[pos1].PKs[pos2].DirBloque != -1)
                {
                    foreach (IndicePrimario_SubBloque ips in DD.Entidades[pos1].PKs[pos2].Sub_Bloque)
                    {
                        dtGrid_Sub_BloquesPK.Rows.Add(ips.Informacion, ips.DirInformacion);
                    }
                }
                else
                {
                    dtGrid_Sub_BloquesPK.Rows.Clear();
                }

            }
        }

        private void txtB_Busqueda_TextChanged(object sender, EventArgs e)
        {
            List<Registro> registrosBusqueda = new List<Registro>();
            int pos = comboEntidadesRegistros.SelectedIndex;
            int pos2 = comboBusqueda.SelectedIndex;
            string cadena = txtB_Busqueda.Text;

            foreach (Registro r in DD.Entidades[pos].Registros)
            {
                string infoBusqueda = Convert.ToString(r.Informacion[pos2]).Substring(0, cadena.Length);
                if (infoBusqueda.Equals(cadena))
                {
                    registrosBusqueda.Add(r);
                }
            }
            dtGrid_Registros.Rows.Clear();
            foreach (Registro reg in registrosBusqueda)
            {
                dtGrid_Registros.Rows.Add();
            }
            for (int i = 0; i < registrosBusqueda.Count; i++)
            {
                dtGrid_Registros[0, i].Value = registrosBusqueda[i].DirRegistro;
                dtGrid_Registros[DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1, i].Value = registrosBusqueda[i].DirSigRegistro;
            }
            for (int i = 0; i < registrosBusqueda.Count; i++)
            {
                for (int j = 1; j < DD.Entidades[comboEntidadesRegistros.SelectedIndex].Atributos.Count + 1; j++)
                {
                    dtGrid_Registros[j, i].Value = registrosBusqueda[i].Informacion[j - 1];
                }
            }
        }

        private void comboBusqueda_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtB_Busqueda.Text = "";
            if (comboBusqueda.SelectedIndex != -1)
            {
                txtB_Busqueda.Enabled = true;
            }
            else
            {
                txtB_Busqueda.Enabled = false;
            }
        }

        private void comboIndice_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int pos = comboIndice.SelectedIndex;
                int posT = comboEntidades.SelectedIndex;
                string indice = comboIndice.Items[comboIndice.SelectedIndex].ToString();

                if (pos != -1 && indice.Equals("3 - CLAVE SECUNDARIA") && posT != -1)
                {
                    comboRelacion.Enabled = true;
                    comboTipoDato.Enabled = false;
                    comboRelacion.Items.Clear();
                    Console.WriteLine(34);
                    foreach (Entidad t in DD.Entidades)
                    {
                        foreach (Atributo at in t.Atributos)
                        {
                            if (at.TipoIndice.Equals(2))
                            {
                                if (!DD.Entidades[posT].Nombre.Equals(t.Nombre))
                                {
                                    comboRelacion.Items.Add(t.Nombre);
                                    break;
                                }
                            }
                        }
                    }
                    comboRelacion.SelectedIndex = -1;
                }
                else
                {
                    comboRelacion.Enabled = false;
                    comboTipoDato.Enabled = true;
                    txtB_NomAtributo.Enabled = true;
                    comboRelacion.Items.Clear();
                    comboEntidades.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboRelacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int posT = comboEntidades.SelectedIndex;
                int cRelacion = comboRelacion.SelectedIndex;
                string busqueda = comboRelacion.Items[cRelacion].ToString();

                if (cRelacion != -1)
                {
                    foreach (Entidad t in DD.Entidades)
                    {
                        if (t.Nombre.Equals(busqueda))
                        {
                            foreach (Atributo at in t.Atributos)
                            {
                                if (at.TipoIndice.Equals(2))
                                {
                                    txtB_NomAtributo.Text = at.Nombre;
                                    if (at.TipoDato.Equals('E'))
                                    {
                                        comboTipoDato.Text = "Entero";
                                        comboTipoDato.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        comboTipoDato.Text = "Cadena";
                                        comboTipoDato.SelectedIndex = 1;
                                    }
                                    txtB_Longitud.Text = Convert.ToString(at.Longitud);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dtGrid_Registros_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine(dtGrid_Registros.CurrentCell.ColumnIndex);
        }

        private void comboHashEstatico_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = comboHashEstatico.SelectedIndex;

            if (pos > -1)
            {
                dtGrid_HashEstatico.Rows.Clear();
                comboCajon.Items.Clear();
                foreach (HashEstatico heb in DD.Entidades[pos].HE)
                {
                    dtGrid_HashEstatico.Rows.Add(heb.DirBloque);
                }
                for (int h = 0; h < 10; h++)
                {
                    comboCajon.Items.Add("L" + h.ToString());
                }
                comboCajon.Text = "";
                dtGrid_Cajon.Rows.Clear();
            }
        }

        private void comboCajon_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = comboHashEstatico.SelectedIndex;
            int pos1 = comboCajon.SelectedIndex;

            if (pos > -1 && pos1 > -1)
            {
                dtGrid_Cajon.Rows.Clear();
                foreach (HashEstatico_SubBloque hesb in DD.Entidades[pos].HE[pos1].Sub_Bloque)
                {
                    dtGrid_Cajon.Rows.Add(hesb.Informacion, hesb.DirInformacion);
                }
            }
        }
    }
}