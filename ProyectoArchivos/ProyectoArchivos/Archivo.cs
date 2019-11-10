using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ProyectoArchivos
{
    class Archivo
    {
        static public long LeeCabezera(string nomRutaArchivo)
        {
            long cab = 0;
            FileStream archivo;
            BinaryReader r;
            try
            {
                archivo = File.Open(nomRutaArchivo, FileMode.Open, FileAccess.Read, FileShare.None);
                archivo.Seek(0, SeekOrigin.Begin);
                r = new BinaryReader(archivo);
                cab = r.ReadInt64();
                archivo.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la lectura de la cabecera:\n" + ex.Message);
            }

            return cab;
        }
        static public void EscribeCabecera(long cab, string nomRutaArchivo)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomRutaArchivo, FileMode.OpenOrCreate)))
                {
                    w.Seek(0, SeekOrigin.Begin);
                    w.Write(cab);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escritura de la cabecera:\n" + ex.Message);
            }
        }

        static public void EscribeEntidad(Entidad e, string nomRutaArchivo)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomRutaArchivo, FileMode.OpenOrCreate)))
                {
                    w.Seek((int)e.DireccionEntidad, SeekOrigin.Begin);
                    w.Write(e.Nombre);
                    w.Write(e.DireccionEntidad);
                    w.Write(e.DireccionAtributo);
                    w.Write(e.DireccionDato);
                    w.Write(e.DireccionSigEntidad);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escribir la entidad:\n" + ex.Message);
            }
        } 
        static public Entidad LeeEntidad(long cabE, string nomRutaArchivo)
        {
            Entidad nuevoEntidad = null;
            Atributo atributoLeido = null;
            long dirSigAux = 0;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomRutaArchivo, FileMode.Open)))
                {
                    r.BaseStream.Seek(cabE, SeekOrigin.Begin);
                    nuevoEntidad = new Entidad();
                    nuevoEntidad.Nombre = r.ReadString();
                    nuevoEntidad.DireccionEntidad = r.ReadInt64();
                    nuevoEntidad.DireccionAtributo = r.ReadInt64();
                    nuevoEntidad.DireccionDato = r.ReadInt64();
                    nuevoEntidad.DireccionSigEntidad = r.ReadInt64();
                    dirSigAux = nuevoEntidad.DireccionAtributo;
                    if (nuevoEntidad.DireccionAtributo != -1)
                    {
                        while (dirSigAux != -1)
                        {
                            atributoLeido = LeeAtributo(dirSigAux, nomRutaArchivo, r);
                            nuevoEntidad.Atributos.Add(atributoLeido);
                            dirSigAux = atributoLeido.DireccionSigAtributo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la lectura de la cabecera:\n" + ex.Message);
            }

            return nuevoEntidad;
        }
        static public List<Entidad> ActualizaEntidades(List<Entidad> entidades, string nomRutaArchivo)
        {
            //Ya me deberia llegar la lista ordenada
            for (int i = 0; i < entidades.Count - 1; i++)
            {
                entidades[i].DireccionSigEntidad = entidades[i + 1].DireccionEntidad;
                EscribeEntidad(entidades[i], nomRutaArchivo);
            }
            foreach (Entidad en in entidades)
            {
                if (en.Atributos.Count > 0)
                {
                    en.DireccionAtributo = en.Atributos[0].DireccionAtributo;
                    EscribeEntidad(en, nomRutaArchivo);
                }
            }

            return entidades;
        }

        static public void EscribeAtributo(Atributo a, string nomRutaArchivo)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomRutaArchivo, FileMode.Open)))
                {
                    w.Seek((int)a.DireccionAtributo, SeekOrigin.Begin);
                    w.Write(a.Nombre);
                    w.Write(a.DireccionAtributo);
                    w.Write(a.TipoDato);
                    w.Write(a.Longitud);
                    w.Write(a.TipoIndice);
                    w.Write(a.TablaRelacion);
                    w.Write(a.DireccionIndice);
                    w.Write(a.DireccionSigAtributo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la escritura de un atributo:\n" + ex.Message);
            }
        }
        static public Atributo LeeAtributo(long cabA, string nomRutaArchivo, BinaryReader r)
        {
            Atributo nuevoAtributo = null;

            try
            {
                r.BaseStream.Seek(cabA, SeekOrigin.Begin);
                nuevoAtributo = new Atributo();
                nuevoAtributo.Nombre = r.ReadString();
                nuevoAtributo.DireccionAtributo = r.ReadInt64();
                nuevoAtributo.TipoDato = r.ReadChar();
                nuevoAtributo.Longitud = r.ReadInt32();
                nuevoAtributo.TipoIndice = r.ReadInt32();
                nuevoAtributo.TablaRelacion = r.ReadString();
                nuevoAtributo.DireccionIndice = r.ReadInt64();
                nuevoAtributo.DireccionSigAtributo = r.ReadInt64();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en lectura de un atributo:\n" + ex.Message);
            }

            return nuevoAtributo;
        }    
        static public List<Atributo> ActualizaAtributos(List<Atributo> atributos, string nomRutaArchivo)
        {
            //Ya me deberia llegar la lista ordenada
            for (int i = 0; i < atributos.Count - 1; i++)
            {
                atributos[i].DireccionSigAtributo = atributos[i + 1].DireccionAtributo;
                EscribeAtributo(atributos[i], nomRutaArchivo);
            }

            return atributos;
        }

        static public string RellenaNombres(string nombre, int n)
        {
            /*El metodo tiene como parametro el nombre que se va a rellenar para despues grabarlo
             * y el tamaño total que se puede grabar en el archivo
             */

            //Variable local que calcula cuanto le falta a el total para agregarle el relleno
            string aux = "";

            for (int i = 0; i < n - nombre.Length; i++)
            {
                aux = aux.Insert(aux.Length, " ");
            }

            return nombre + aux;
        }
        static public string DecimalaBinario(long numero)
        {
            string cadena = "";
            long diferencia = 0;

            if (numero > 0)
            {
                while (numero > 0)
                {
                    if (numero % 2 == 0)
                    {
                        cadena = "0" + cadena;
                    }
                    else
                    {
                        cadena = "1" + cadena;
                    }
                    numero = (int)(numero / 2);
                }

            }
            else
            {
                if (numero == 0)
                {
                    cadena = "0" + cadena;
                }
                else
                {
                    MessageBox.Show("Ingrese solo numeros positivos");
                }
            }
            diferencia = 6 - cadena.Length;
            for (int i = 0; i < diferencia; i++)
            {
                cadena = "0" + cadena;
            }

            return cadena;
        }
        static public string DecimalaBinariDigitos(long n)
        {
            string cadena1 = "";
            string cadena2 = "";
            int diferencia = 0;
            string aux = n.ToString();
            int n0 = Convert.ToInt32(aux[0]) - 48;
            int n1 = Convert.ToInt32(aux[1]) - 48;

            cadena1 = Convert.ToString(n0, 2);
            cadena2 = Convert.ToString(n1, 2);
            diferencia = 4 - cadena1.Length;
            for (int i = 0; i < diferencia; i++)
            {
                cadena1 = "0" + cadena1;
            }
            diferencia = 4 - cadena2.Length;
            for (int i = 0; i < diferencia; i++)
            {
                cadena2 = "0" + cadena2;
            }

            return cadena1 + cadena2;
        }

        static public void CreaArchivoDat(string nomDat)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomDat, FileMode.Create)))
                {
                    w.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en crear el archivo .dat:\n" + ex.Message);
            }
        }
        static public void CreaArchivoIdx(string nomIdx)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIdx, FileMode.Create)))
                {
                    w.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en crear el archivo .idx:\n" + ex.Message);
            }
        }

        static public void EscribeRegistro(Registro reg, Entidad en, string nomRegistro)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomRegistro, FileMode.Open)))
                {
                    w.Seek((int)reg.DirRegistro, SeekOrigin.Begin);
                    w.Write(reg.DirRegistro);
                    for (int i = 0; i < en.Atributos.Count; i++)
                    {
                        switch (en.Atributos[i].TipoDato)
                        {
                            case 'E':
                                w.Write(Convert.ToInt32(reg.Informacion[i]));
                                break;
                            case 'C':
                                w.Write(Convert.ToString(reg.Informacion[i]));
                                break;
                        }
                    }
                    w.Write(reg.DirSigRegistro);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escribir el registro:\n" + ex.Message);
            }
        }
        static public Registro LeeRegistro(long cabR, Entidad en, string nomRegistro)
        {
            Registro rAux = null;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomRegistro, FileMode.Open)))
                {
                    rAux = new Registro();

                    r.BaseStream.Seek(cabR, SeekOrigin.Begin);
                    rAux.DirRegistro = r.ReadInt64();
                    foreach (Atributo at in en.Atributos)
                    {
                        switch (at.TipoDato)
                        {
                            case 'E':
                                int aux = r.ReadInt32();
                                rAux.Informacion.Add(aux.ToString());
                                break;
                            case 'C':
                                rAux.Informacion.Add(r.ReadString());
                                break;
                        }
                    }
                    rAux.DirSigRegistro = r.ReadInt64();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la lectura de un registro:\n" + ex.Message);
            }
            
            return rAux;
        }
        static public List<Registro> ActualizaRegistros(List<Registro> registros, Entidad en, string nomRutaArchivo)
        {
            //Ya me deberia llegar la lista ordenada
            if (registros.Count > 1)
            {
                for (int i = 0; i < registros.Count - 1; i++)
                {
                    registros[i].DirSigRegistro = registros[i + 1].DirRegistro;
                    EscribeRegistro(registros[i], en, nomRutaArchivo);
                }
                registros[registros.Count - 1].DirSigRegistro = -1;
                EscribeRegistro(registros[registros.Count - 1], en, nomRutaArchivo);

            }
            else if (registros.Count == 1)
            {
                registros[registros.Count - 1].DirSigRegistro = -1;
                EscribeRegistro(registros[registros.Count - 1], en, nomRutaArchivo);
            }


            return registros;
        }

        static public List<IndicePrimario> InicializaIndicePrimario(long dir, Atributo at, string nomIndice)
        {
            List<IndicePrimario> indices = new List<IndicePrimario>();
            long dirCont = dir;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dir, SeekOrigin.Begin);
                    switch (at.TipoDato)
                    {
                        case 'C':
                            for (int i = 65; i < 91; i++)
                            {
                                IndicePrimario ip = new IndicePrimario();

                                ip.DirIndice = dirCont;
                                ip.ID = Convert.ToChar(i);
                                ip.Sub_Bloque = null;
                                ip.DirBloque = -1;

                                w.Write(Convert.ToChar(ip.ID));
                                w.Write(ip.DirBloque);

                                indices.Add(ip);
                                dirCont += 9;
                            }
                            break;
                        case 'E':
                            for (int i = 1; i < 10; i++)
                            {
                                IndicePrimario ip = new IndicePrimario();

                                ip.DirIndice = dirCont;
                                ip.ID = i;
                                ip.Sub_Bloque = null;
                                ip.DirBloque = -1;

                                w.Write(Convert.ToInt32(i));
                                w.Write(ip.DirBloque);

                                indices.Add(ip);
                                dirCont += 12;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del indice primario:\n" + ex.Message);
            }

            return indices;
        }
        static public List<IndicePrimario_SubBloque> InicializaSub_BloqueIndicePrimario(long dir, Atributo at, string nomIndice)
        {
            List<IndicePrimario_SubBloque> diccionario = null;
            long dirCont = dir;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    diccionario = new List<IndicePrimario_SubBloque>(100);

                    w.Seek((int)dir, SeekOrigin.Begin);
                    for (int i = 0; i < 100; i++)
                    {
                        IndicePrimario_SubBloque ips = new IndicePrimario_SubBloque();
                        switch (at.TipoDato)
                        {
                            case 'E':
                                ips.DirSubBloque = dirCont;
                                ips.Informacion = 0;
                                ips.DirInformacion = -1;

                                w.Write(Convert.ToInt32(ips.Informacion));
                                w.Write(ips.DirInformacion);

                                diccionario.Add(ips);
                                dirCont += at.Longitud + 8;
                                break;
                            case 'C':
                                ips.DirSubBloque = dirCont;
                                ips.Informacion = RellenaNombres("", at.Longitud - 1);
                                ips.DirInformacion = -1;

                                w.Write(Convert.ToString(ips.Informacion));
                                w.Write(ips.DirInformacion);

                                diccionario.Add(ips);
                                dirCont += at.Longitud + 8;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del un sub-bloqu del indice primario:\n" + ex.Message);
            }

            return diccionario;
        }
        static public void EscribeIndicePrimario(IndicePrimario ip, Atributo at, string nomIndice)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)ip.DirIndice, SeekOrigin.Begin);
                    switch (at.TipoDato)
                    {
                        case 'E':
                            w.Write(Convert.ToInt32(ip.ID));
                            w.Write(ip.DirBloque);
                            break;
                        case 'C':
                            w.Write(Convert.ToChar(ip.ID));
                            w.Write(ip.DirBloque);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escribir el indice primario:\n" + ex.Message);
            }
        }
        static public void EscribeSub_BloqueIndicePrimario(List<IndicePrimario_SubBloque> ipSB, Atributo at, string nomIndice)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    foreach (IndicePrimario_SubBloque ips in ipSB)
                    {
                        w.Seek((int)ips.DirSubBloque, SeekOrigin.Begin);
                        switch (at.TipoDato)
                        {
                            case 'E':
                                w.Write(Convert.ToInt32(ips.Informacion));
                                break;
                            case 'C':
                                w.Write(Convert.ToString(ips.Informacion));
                                break;
                        }
                        w.Write(ips.DirInformacion);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escribir el indice primario:\n" + ex.Message);
            }
        }
        static public IndicePrimario LeeIndicePrimario(long dir, Atributo at, string nomIndice)
        {
            IndicePrimario ipAux = null;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomIndice, FileMode.Open)))
                {
                    ipAux = new IndicePrimario();

                    r.BaseStream.Seek(dir, SeekOrigin.Begin);
                    ipAux.DirIndice = dir;
                    if (at.TipoDato == 'C')
                    {
                        ipAux.ID = r.ReadChar();
                    }
                    else if (at.TipoDato == 'E')
                    {
                        ipAux.ID = r.ReadInt32();
                    }
                    ipAux.DirBloque = r.ReadInt64();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en leer indice primario:\n" + ex.Message);
            }

            return ipAux;
        }
        static public List<IndicePrimario_SubBloque> LeeSub_BloqueIndicePrimario(long dir, Atributo at, string nomIndice)

        {
            List<IndicePrimario_SubBloque> dicAux = null;
            long dirCont = dir;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomIndice, FileMode.Open)))
                {
                    dicAux = new List<IndicePrimario_SubBloque>(100);

                    for (int i = 0; i < 100; i++)
                    {
                        IndicePrimario_SubBloque ips = new IndicePrimario_SubBloque();
                        r.BaseStream.Seek(dirCont, SeekOrigin.Begin);
                        switch (at.TipoDato)
                        {
                            case 'E':
                                ips.DirSubBloque = dirCont;
                                ips.Informacion = r.ReadInt32();
                                ips.DirInformacion = r.ReadInt64();
                                dicAux.Add(ips);
                                dirCont += at.Longitud + 8;

                                break;
                            case 'C':
                                ips.DirSubBloque = dirCont;
                                ips.Informacion = r.ReadString();
                                ips.DirInformacion = r.ReadInt64();
                                dicAux.Add(ips);
                                dirCont += at.Longitud + 8;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en leer un sub-bloque indice primario:\n" + ex.Message);
            }

            return dicAux;
        }
        static public List<IndicePrimario_SubBloque> OrdenaSubIndicesPrimarios(List<IndicePrimario_SubBloque> subs, Atributo at)
        {
            List<IndicePrimario_SubBloque> listAux = new List<IndicePrimario_SubBloque>();

            foreach (IndicePrimario_SubBloque iS in subs)
            {
                switch (at.TipoDato)
                {
                    case 'E':
                        if (Convert.ToInt32(iS.Informacion) != 0)
                        {
                            IndicePrimario_SubBloque aux = new IndicePrimario_SubBloque();
                            aux.Informacion = iS.Informacion;
                            aux.DirInformacion = iS.DirInformacion;
                            listAux.Add(aux);
                        }
                        break;
                    case 'C':
                        if (!Convert.ToString(iS.Informacion).Equals(RellenaNombres("", at.Longitud - 1)))
                        {
                            IndicePrimario_SubBloque aux = new IndicePrimario_SubBloque();
                            aux.Informacion = iS.Informacion;
                            aux.DirInformacion = iS.DirInformacion;
                            listAux.Add(aux);
                        }
                        break;
                }
            }
            listAux = listAux.OrderBy(ord => ord.Informacion).ToList();
            for (int i = 0; i < subs.Count; i++)
            {
                if (i < listAux.Count)
                {
                    subs[i].Informacion = listAux[i].Informacion;
                    subs[i].DirInformacion = listAux[i].DirInformacion;
                }
                else
                {
                    if (at.TipoDato == 'E')
                        subs[i].Informacion = 0;
                    else if (at.TipoDato == 'C')
                        subs[i].Informacion = RellenaNombres("", at.Longitud - 1);
                    subs[i].DirInformacion = -1;
                }
            }

            return subs;
        }

        static public List<IndiceSecundario> InicializaIndiceSecundario(long dir, Atributo at, string nomIndice)
        {
            List<IndiceSecundario> indices = new List<IndiceSecundario>();
            long dirCont = dir;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dir, SeekOrigin.Begin);
                    switch (at.TipoDato)
                    {
                        case 'C':
                            for (int i = 0; i < 50; i++)
                            {
                                IndiceSecundario iS = new IndiceSecundario();

                                iS.DirIndice = dirCont;
                                iS.ID = Convert.ToString(RellenaNombres("", at.Longitud - 1));
                                iS.Sub_Bloque = null;
                                iS.DirBloque = -1;

                                w.Write(Convert.ToString(RellenaNombres("", at.Longitud - 1)));
                                w.Write(iS.DirBloque);

                                indices.Add(iS);
                                dirCont += at.Longitud + 8;
                            }
                            break;
                        case 'E':
                            for (int i = 0; i < 50; i++)
                            {
                                IndiceSecundario iS = new IndiceSecundario();

                                iS.DirIndice = dirCont;
                                iS.ID = Convert.ToInt32(0);
                                iS.Sub_Bloque = null;
                                iS.DirBloque = -1;

                                w.Write(Convert.ToInt32(0));
                                w.Write(iS.DirBloque);

                                indices.Add(iS);
                                dirCont += at.Longitud + 8;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del indice secundario:\n" + ex.Message);
            }

            return indices;
        }
        static public List<IndiceSecundario_SubBloque> InicializaSub_BloqueIndiceSecundario(long dir, Atributo at, string nomIndice)
        {
            List<IndiceSecundario_SubBloque> diccionario = null;
            long dirCont = dir;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    diccionario = new List<IndiceSecundario_SubBloque>(100);

                    w.Seek((int)dir, SeekOrigin.Begin);
                    for (int i = 0; i < 100; i++)
                    {
                        IndiceSecundario_SubBloque iSs = new IndiceSecundario_SubBloque();
                        switch (at.TipoDato)
                        {
                            case 'E':
                                iSs.DirSubBloque = dirCont;
                                iSs.DirInformacion = -1;

                                w.Write(iSs.DirInformacion);

                                diccionario.Add(iSs);
                                dirCont += 8;
                                break;
                            case 'C':
                                iSs.DirSubBloque = dirCont;
                                iSs.DirInformacion = -1;

                                w.Write(iSs.DirInformacion);

                                diccionario.Add(iSs);
                                dirCont += 8;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del un sub-bloqu del indice secundario:\n" + ex.Message);
            }

            return diccionario;
        }
        static public void EscribeIndiceSecundario(IndiceSecundario iS, Atributo at, string nomIndice)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)iS.DirIndice, SeekOrigin.Begin);
                    switch (at.TipoDato)
                    {
                        case 'E':
                            w.Write(Convert.ToInt32(iS.ID));
                            w.Write(iS.DirBloque);
                            break;
                        case 'C':
                            w.Write(Convert.ToString(iS.ID));
                            w.Write(iS.DirBloque);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escribir el indice secundario:\n" + ex.Message);
            }
        }
        static public void EscribeSub_BloqueIndiceSecundario(List<IndiceSecundario_SubBloque> isSB, Atributo at, string nomIndice)
        {
            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    foreach (IndiceSecundario_SubBloque ips in isSB)
                    {
                        w.Seek((int)ips.DirSubBloque, SeekOrigin.Begin);
                        switch (at.TipoDato)
                        {
                            case 'E':
                                w.Write(ips.DirInformacion);
                                break;
                            case 'C':
                                w.Write(ips.DirInformacion);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en escribir el indice secundario:\n" + ex.Message);
            }
        }
        static public IndiceSecundario LeeIndiceSecundario(long dir, Atributo at, string nomIndice)
        {
            IndiceSecundario isAux = null;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomIndice, FileMode.Open)))
                {
                    isAux = new IndiceSecundario();

                    r.BaseStream.Seek(dir, SeekOrigin.Begin);
                    isAux.DirIndice = dir;
                    if (at.TipoDato == 'C')
                    {
                        isAux.ID = r.ReadString();
                    }
                    else if (at.TipoDato == 'E')
                    {
                        isAux.ID = r.ReadInt32();
                    }
                    isAux.DirBloque = r.ReadInt64();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en leer indice secundario:\n" + ex.Message);
            }

            return isAux;
        }
        static public List<IndiceSecundario_SubBloque> LeeSub_BloqueIndiceSecundario(long dir, Atributo at, string nomIndice)
        {
            List<IndiceSecundario_SubBloque> dicAux = null;
            long dirCont = dir;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomIndice, FileMode.Open)))
                {
                    dicAux = new List<IndiceSecundario_SubBloque>(100);

                    for (int i = 0; i < 100; i++)
                    {
                        IndiceSecundario_SubBloque ips = new IndiceSecundario_SubBloque();
                        r.BaseStream.Seek(dirCont, SeekOrigin.Begin);
                        switch (at.TipoDato)
                        {
                            case 'E':
                                ips.DirSubBloque = dirCont;
                                ips.DirInformacion = r.ReadInt64();
                                dicAux.Add(ips);
                                dirCont += 8;
                                break;
                            case 'C':
                                ips.DirSubBloque = dirCont;
                                ips.DirInformacion = r.ReadInt64();
                                dicAux.Add(ips);
                                dirCont += 8;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en leer un sub-bloque indice secundario:\n" + ex.Message);
            }

            return dicAux;
        }
        static public List<IndiceSecundario> OrdenaIndiceSecundario(List<IndiceSecundario> subs, Atributo at, string nomIndice)
        {
            List<IndiceSecundario> listAux = new List<IndiceSecundario>();

            
            foreach (IndiceSecundario iS in subs)
            {
                switch (at.TipoDato)
                {
                    case 'E':
                        if (Convert.ToInt32(iS.ID) != 0)
                        {
                            IndiceSecundario aux = new IndiceSecundario();
                            aux.ID = iS.ID;
                            aux.DirBloque = iS.DirBloque;
                            listAux.Add(aux);
                        }
                        break;
                    case 'C':
                        if (!Convert.ToString(iS.ID).Equals(RellenaNombres("", at.Longitud - 1)))
                        {
                            IndiceSecundario aux = new IndiceSecundario();
                            aux.ID = iS.ID;
                            aux.DirBloque = iS.DirBloque;
                            listAux.Add(aux);
                        }
                        break;
                }
            }
            listAux = listAux.OrderBy(ord => ord.ID).ToList();
            for (int i = 0; i < subs.Count; i++)
            {
                if (i < listAux.Count)
                {
                    subs[i].ID = listAux[i].ID;
                    subs[i].DirBloque = listAux[i].DirBloque;
                }
                else
                {
                    if (at.TipoDato == 'E')
                        subs[i].ID = 0;
                    else if (at.TipoDato == 'C')
                        subs[i].ID = RellenaNombres("", at.Longitud - 1);
                    subs[i].DirBloque = -1;
                }
                EscribeIndiceSecundario(subs[i], at, nomIndice);
            }

            return subs;
        }
        static public List<IndiceSecundario_SubBloque> OrdenaSubIndicesSecundarios(List<IndiceSecundario_SubBloque> subs, Atributo at)
        {
            List<IndiceSecundario_SubBloque> listAux = new List<IndiceSecundario_SubBloque>();

            foreach (IndiceSecundario_SubBloque iS in subs)
            {
                switch (at.TipoDato)
                {
                    case 'E':
                        if (Convert.ToInt32(iS.DirInformacion) != -1)
                        {
                            IndiceSecundario_SubBloque aux = new IndiceSecundario_SubBloque();
                            aux.DirInformacion = iS.DirInformacion;
                            listAux.Add(aux);
                        }
                        break;
                    case 'C':
                        if (Convert.ToInt32(iS.DirInformacion) != -1)
                        {
                            IndiceSecundario_SubBloque aux = new IndiceSecundario_SubBloque();
                            aux.DirInformacion = iS.DirInformacion;
                            listAux.Add(aux);
                        }
                        break;
                }
            }
            listAux = listAux.OrderBy(ord => ord.DirInformacion).ToList();
            for (int i = 0; i < subs.Count; i++)
            {
                if (i < listAux.Count)
                {
                    subs[i].DirInformacion = listAux[i].DirInformacion;
                }
                else
                {
                    subs[i].DirInformacion = -1;
                }
            }

            return subs;
        }

        static public HashDinamico InicializaHashDinamico(long dir, string nomIndice)
        {
            HashDinamico hAux = new HashDinamico();
            long dirCont = dir + 4;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dir, SeekOrigin.Begin);

                    hAux.ID = 0;
                    w.Write(hAux.ID);
                    for (int i = 0; i < 64; i++)
                    {
                        HashDinamico_Bloque aux2 = new HashDinamico_Bloque();

                        aux2.DirHash = dirCont;
                        aux2.CodigoB = DecimalaBinario((long)i); 
                        aux2.DirSub_Bloque = -1;
                        aux2.Sub_Bloque = null;
                        foreach (char s in aux2.CodigoB)
                        {
                            w.Write(s);
                        }
                        w.Write(aux2.DirSub_Bloque);

                        hAux.BloquePrincipal.Add(aux2);
                        dirCont += 6 + 8;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del hash dinamico:\n" + ex.Message);
            }

            return hAux;
        }
        static public HashDinamico_SubBloque InicializaHashDinamicoSubBloque(long dir, string nomIndice)
        {
            HashDinamico_SubBloque hsb = new HashDinamico_SubBloque();
            List<HashDinamico_SubBloqueInformacion> hsbAux = new List<HashDinamico_SubBloqueInformacion>();
            long dirCont = dir + 4;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dir, SeekOrigin.Begin);

                    hsb.ID = 0;
                    hsb.DirDesbordamiento = -1;
                    hsb.ListaDesbordamiento = null;

                    w.Write(hsb.ID);
                    for (int i = 0; i < 4; i++)//fgbffrg
                    {
                        HashDinamico_SubBloqueInformacion aux2 = new HashDinamico_SubBloqueInformacion();

                        aux2.DirSub_Bloque = dirCont;
                        aux2.Valor = 0;
                        aux2.DirRegistro = -1;
                        
                        w.Write(aux2.Valor);
                        w.Write(aux2.DirRegistro);

                        hsbAux.Add(aux2);

                        dirCont += 12;
                    }
                    w.Write(hsb.DirDesbordamiento);
                    hsb.HashInformacion = hsbAux;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del hash dinamico sub-bloque:\n" + ex.Message);
            }
            return hsb;
        }
        static public void EscribeHashDinamicoBloque(long dirinicio, HashDinamico hs, string nomIndice)
        {
            long dirCont = dirinicio;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dirCont, SeekOrigin.Begin);

                    w.Write(hs.ID);
                    for (int i = 0; i < 64; i++)
                    {
                        w.Seek((int)hs.BloquePrincipal[i].DirHash, SeekOrigin.Begin);
                        foreach (char s in hs.BloquePrincipal[i].CodigoB)
                        {
                            w.Write(s);
                        }
                        w.Write(hs.BloquePrincipal[i].DirSub_Bloque);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la escibir del hash dinamico, bloque principal:\n" + ex.Message);
            }
        }
        static public void EscribeHashDinamicoSubBloque(long dirinicio, HashDinamico_SubBloque hs, string nomIndice)
        {
            long dirCont = dirinicio + 4;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dirinicio, SeekOrigin.Begin);

                    w.Write(hs.ID);
                    for (int i = 0; i < 4; i++)//Debes de cambiarlo a los que quieras
                    {
                        w.Seek((int)dirCont, SeekOrigin.Begin);
                        w.Write(hs.HashInformacion[i].Valor);
                        w.Write(hs.HashInformacion[i].DirRegistro);
                        dirCont += 12;
                    }
                    w.Write(hs.DirDesbordamiento);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la escibir del hash dinamico, bloque principal:\n" + ex.Message);
            }
        }
        static public HashDinamico LeeHashDinamicoBloque(long dir, string nomIndice)
        {
            HashDinamico hs = null;
            long dirCont = dir + 4;

            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomIndice, FileMode.Open)))
                {
                    hs = new HashDinamico();

                    r.BaseStream.Seek((int)dir, SeekOrigin.Begin);
                    hs.ID = r.ReadInt32();
                    for (int i = 0; i < 64; i++)
                    {
                        r.BaseStream.Seek((int)dirCont, SeekOrigin.Begin);
                        HashDinamico_Bloque aux = new HashDinamico_Bloque();

                        aux.DirHash = dirCont;
                        for (int h = 0; h < 6; h++)
                        {
                            aux.CodigoB += r.ReadChar();
                        }
                        aux.DirSub_Bloque = r.ReadInt64();
                        hs.BloquePrincipal.Add(aux);
                        dirCont += 14;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la lectura de un hash dinamico bloque:\n" + ex.Message);
            }


            return hs;
        }
        static public HashDinamico_SubBloque LeeHashDinamicoSubBloque(long dir, string nomIndice)
        {
            HashDinamico_SubBloque hssb = null;
            long dirCont = dir + 4;
            
            try
            {
                using (BinaryReader r = new BinaryReader(File.Open(nomIndice, FileMode.Open)))
                {
                    hssb = new HashDinamico_SubBloque();

                    r.BaseStream.Seek(dir, SeekOrigin.Begin);
                    hssb.ID = r.ReadInt32();
                    hssb.HashInformacion = new List<HashDinamico_SubBloqueInformacion>();
                    for (int i = 0; i < 4; ++i)//Cambiar
                    {
                        HashDinamico_SubBloqueInformacion aux2 = new HashDinamico_SubBloqueInformacion();
                        
                        aux2.DirSub_Bloque = dirCont;
                        aux2.Valor = r.ReadInt32();
                        aux2.DirRegistro = r.ReadInt64();
                        hssb.HashInformacion.Add(aux2);
                        dirCont += 12;
                    }
                    hssb.DirDesbordamiento = r.ReadInt64();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la lectura de un hash dinamico sub-bloque:\n" + ex.Message);
            }


            return hssb;
        }

        static public List<HashEstatico> InicializaHashEstatico(long dir, string nomIndice)
        {
            List<HashEstatico> hash = new List<HashEstatico>();
            long dirCont = dir;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    w.Seek((int)dir, SeekOrigin.Begin);
                    for (int i = 0; i < 10; i++)
                    {
                        HashEstatico heb = new HashEstatico();

                        heb.DirIndice = dirCont;
                        heb.Sub_Bloque = null;
                        heb.DirBloque = -1;
                        heb.Desbordamiento = -1;

                        w.Write(heb.DirBloque);

                        hash.Add(heb);
                        dirCont += 8;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del indice secundario:\n" + ex.Message);
            }

            return hash;
        }
        static public List<HashEstatico_SubBloque> InicializaSub_BloqueInicializaHashEstatico(long dir, Atributo at, HashEstatico he, string nomIndice)
        {
            List<HashEstatico_SubBloque> subBloque = null;
            long dirCont = dir;

            try
            {
                using (BinaryWriter w = new BinaryWriter(new FileStream(nomIndice, FileMode.Open)))
                {
                    subBloque = new List<HashEstatico_SubBloque>();

                    w.Seek((int)dir, SeekOrigin.Begin);
                    for (int i = 0; i < 14; i++)
                    {
                        HashEstatico_SubBloque hesb = new HashEstatico_SubBloque();
                        switch (at.TipoDato)
                        {
                            case 'E':
                                hesb.DirSubBloque = dirCont;
                                hesb.Informacion = Convert.ToInt32(-1);
                                hesb.DirInformacion = -1;

                                w.Write(Convert.ToInt32(hesb.Informacion));
                                w.Write(hesb.DirInformacion);

                                subBloque.Add(hesb);
                                dirCont += 12;
                                break;
                            case 'C':
                                hesb.DirSubBloque = dirCont;
                                hesb.Informacion = RellenaNombres("", at.Longitud - 1);
                                hesb.DirInformacion = -1;

                                w.Write(Convert.ToString(hesb.Informacion));
                                w.Write(hesb.DirInformacion);

                                subBloque.Add(hesb);
                                dirCont += at.Longitud + 8;
                                break;
                        }
                    }
                    w.Write(he.Desbordamiento);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema en la inicialización del un sub-bloque del hash estatico:\n" + ex.Message);
            }

            return subBloque;
        }


    }
} 