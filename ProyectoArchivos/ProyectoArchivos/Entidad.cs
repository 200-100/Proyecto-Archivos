using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class Entidad
    {
        //Variable de tipo String que guarda el nombre de la entidad
        private string nomEntidad;
        //Variable de tipo long que guarda la direccion de la entidad posicionada en el archivo binario
        private long dirEntidad;
        //Variable de tipo long que guarda la direccion de el primer atributo de la entidad
        private long dirPAtributo;
        //Variable de tipo long que guarda la direccion de el dato de la entidad
        private long dirDato;
        //Variable de tipo long que guarda la direccion de la siguiente entidad
        private long dirSigEntidad;
        //
        private List<Atributo> atributos = new List<Atributo>();
        //
        private List<Registro> registros = new List<Registro>();
        //
        private List<IndicePrimario> pks = new List<IndicePrimario>();
        //
        private List<List<IndiceSecundario>> fks = new List<List<IndiceSecundario>>();
        //
        private HashDinamico hs = new HashDinamico();

        //Constructor
        public Entidad()
        {
        }
        //Metodo que regresa y actualiza el nombre de la entidad
        public string Nombre
        {
            get { return nomEntidad; }
            set { nomEntidad = value; }
        }
        //Metodo que regresa y actualiza la direccion de la entidad
        public long DireccionEntidad
        {
            get { return dirEntidad; }
            set { dirEntidad = value; }
        }
        //Metodo que regresa y actualiza la direccion de el primer atributo de la entidad
        public long DireccionAtributo
        {
            get { return dirPAtributo; }
            set { dirPAtributo = value; }
        }
        //Metodo que regresa y actualiza la direccion de el dato de la entidad
        public long DireccionDato
        {
            get { return dirDato; }
            set { dirDato = value; }
        }
        //Metodo que regresa y actualiza la direccion de el dato de la entidad
        public long DireccionSigEntidad
        {
            get { return dirSigEntidad; }
            set { dirSigEntidad = value; }
        }
        //
        public List<Atributo> Atributos
        {
            get { return atributos; }
            set { atributos = value; }
        }
        //
        public List<Registro> Registros
        {
            get { return registros; }
            set { registros = value; }
        }
        //
        public List<IndicePrimario> PKs
        {
            get { return pks; }
            set { pks = value; }
        }
        //
        public List<List<IndiceSecundario>> FKs
        {
            get { return fks; }
            set { fks = value; }
        }
        //
        public HashDinamico HS
        {
            get { return hs; }
            set { hs = value; }
        }
    }
}