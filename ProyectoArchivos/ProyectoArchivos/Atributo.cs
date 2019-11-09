using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class Atributo
    {
        //Variable de tipo String que guarda el nombre de el atributo
        private string nomAtributo;
        //Variable de tipo long que guarda la direccion de el atributo posicionada en el archivo binario
        private long dirAtributo;
        //Variable de tipo char que guarda el tipo de dato de el atributo
        private char tipoDato;
        //Variable de típo entero que guarda el tamaño de la cadena de su nombre
        private int longitud;
        //Variable de tipo entero que guarda el tipo de indice del atributo
        private int tipoIndice;
        //Variable de tipo long que guarda la direccion de el indice
        private long dirIndice;
        private string relacion;
        //Variable de tipo long que guarda la direccion de la siguiente entidad
        private long dirSigAtributo;

        //Constructor
        public Atributo()
        {
        }
        //Metodo que regresa y actualiza el nombre de el atributo
        public string Nombre
        {
            get { return nomAtributo; }
            set { nomAtributo = value; }
        }
        //Metodo que regresa y actualiza la direccion del atributo
        public long DireccionAtributo
        {
            get { return dirAtributo; }
            set { dirAtributo = value; }
        }
        //Metodo que regresa y actualiza el tipo de dato
        public char TipoDato
        {
            get { return tipoDato; }
            set { tipoDato = value; }
        }
        //Metodo que regresa y actualiza la longitud de la cadena del nombre
        public int Longitud
        {
            get { return longitud; }
            set { longitud = value; }
        }
        //Metodo que regresa y actualiza el tipo de indice del atributo
        public int TipoIndice
        {
            get { return tipoIndice; }
            set { tipoIndice = value; }
        }
        //Metodo que regresa y actualiza la direccion del indice
        public long DireccionIndice
        {
            get { return dirIndice; }
            set { dirIndice = value; }
        }
        //Metodo que regresa y actualiza la direccion del siguiente atributo
        public long DireccionSigAtributo
        {
            get { return dirSigAtributo; }
            set { dirSigAtributo = value; }
        }
        public string TablaRelacion
        {
            get { return relacion; }
            set { relacion = value; }
        }
    }
}
