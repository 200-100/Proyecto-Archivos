using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class HashDinamico
    {
        //Variable de tipo int que guarda la cabecera de el bloque
        private int id;
        //Lista de clase HashDinamico_Bloque que guarda los identificadores binarios y de ahí apunta a la información
        private List<HashDinamico_Bloque> bloqueprincipal = new List<HashDinamico_Bloque>();

        //CONSTRUCTOR
        public HashDinamico()
        {
        }
        //Metodo que regresa y actualiza la cabecera de el bloque
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        //Metodo que regresa y actualiza los identificadores binarios y de ahí apunta a la información
        public List<HashDinamico_Bloque> BloquePrincipal
        {
            get { return bloqueprincipal; }
            set { bloqueprincipal = value; }
        }
    }
}