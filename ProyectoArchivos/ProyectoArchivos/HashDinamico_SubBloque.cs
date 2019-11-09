using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class HashDinamico_SubBloque
    {
        //Variable de tipo int que guarda la cabecera de el sub-bloque
        private int id;
        //Lista de tipo 
        private List<HashDinamico_SubBloqueInformacion> hashinformacion;
        //
        private long dirdesbordamiento;
        //
        private HashDinamico_SubBloque listadesbordamiento;

        //CONSTRUCTOR
        public HashDinamico_SubBloque()
        {
        }
        //
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        //
        public List<HashDinamico_SubBloqueInformacion> HashInformacion
        {
            get { return hashinformacion; }
            set { hashinformacion = value; }
        }
        //
        public long DirDesbordamiento
        {
            get { return dirdesbordamiento; }
            set { dirdesbordamiento = value; }
        }
        //
        public HashDinamico_SubBloque ListaDesbordamiento
        {
            get { return listadesbordamiento; }
            set { listadesbordamiento = value; }
        }
    }
}
