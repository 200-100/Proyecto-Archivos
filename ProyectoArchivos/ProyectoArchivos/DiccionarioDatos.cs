using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class DiccionarioDatos
    {
        //Lista de clase Entidad que guarda las entidades de el diccionario
        private List<Entidad> entidades = new List<Entidad>();
        //
        private long cabecera;

        //Constructor
        public DiccionarioDatos()
        {
        }
        //Metodo que regresa y actualiza la lista de entidades en el diccionario
        public List<Entidad> Entidades
        {
            get { return entidades; }
            set { entidades = value; }
        }
        //
        public long Cabecera
        {
            get { return cabecera; }
            set { cabecera = value; }
        }
    }
}
