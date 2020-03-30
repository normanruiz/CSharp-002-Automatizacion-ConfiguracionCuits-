using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.ServiceProcess;

namespace reconfiguraCuits
{
    static class Procesos
    {

        public static List<String> LoteDeCarga()
        {
            Configuracion configuracion = new Configuracion(Directory.GetCurrentDirectory() + @"\config\cuitsConfig.xml");
            List<String> listadoAux = new List<string>();
            try
            {
                Console.Write("\tVerificando directorio... ");
                String rutaCUITs = configuracion.rutaCuits;
                DirectoryInfo directorio = new DirectoryInfo(rutaCUITs);
                foreach (var cuit in directorio.GetDirectories()) { listadoAux.Add(cuit.Name); };
                Console.WriteLine("Ok.");
                Console.WriteLine();
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
            return listadoAux;
        }

        public static Boolean LoteDeProceso(List<String> listadoAux)
        {
            Configuracion configuracion = new Configuracion(Directory.GetCurrentDirectory() + @"\config\cuitsConfig.xml");
            Boolean estadoAux = false;
            try
            {
                Console.Write("\tProcesando directorio... ");
                String rutaCUITs = configuracion.rutaCuits;
                foreach (var cuit in listadoAux)
                {
                    Procesos.ConfiguraDirectorio(rutaCUITs + cuit);
                    if (Procesos.CreaArchivo(cuit))
                    {
                        estadoAux = true;
                    }
                };
                Console.WriteLine("Ok.");
                Console.WriteLine();
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
            return estadoAux;
        }

        public static Boolean ConfiguraDirectorio(String cuitAux)
        {
            Configuracion configuracion = new Configuracion(Directory.GetCurrentDirectory() + @"\config\cuitsConfig.xml");
            Boolean estadoAux = false;
            try
            {
                if (!(Directory.Exists(cuitAux + @"\procesados")))
                {
                    Directory.CreateDirectory(cuitAux + @"\procesados");
                    estadoAux = true;
                }
                if (!(Directory.Exists(cuitAux + @"\errores")))
                {
                    Directory.CreateDirectory(cuitAux + @"\errores");
                    estadoAux = true;
                }
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
            return estadoAux;
        }

        public static Boolean CreaArchivo(String cuitAux)
        {
            Configuracion configuracion = new Configuracion(Directory.GetCurrentDirectory() + @"\config\cuitsConfig.xml");
            Boolean estadoAux = false;
            String nombreArchivo = "TR_" + cuitAux + ".xml";
            String rutaArchivo = configuracion.rutaTR;
            String rutaCuits = configuracion.rutaCuits;
            StreamWriter archivoTR = null;
            try
            {
                if (!(File.Exists(rutaArchivo + nombreArchivo)))
                {
                    archivoTR = new StreamWriter(rutaArchivo + nombreArchivo);
                    archivoTR.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                    archivoTR.WriteLine("<ConfiguracionArchivo>");
                    archivoTR.WriteLine("   <entidad>" + cuitAux + "</entidad>");
                    archivoTR.WriteLine("   <carpetaDeInput>" + rutaCuits + cuitAux + @"\</carpetaDeInput>");
                    archivoTR.WriteLine("   <carpetaDeProcesados>" + rutaCuits + cuitAux + @"\procesados\</carpetaDeProcesados>");
                    archivoTR.WriteLine("   <carpetaDeErrores>" + rutaCuits + cuitAux + @"\errores\</carpetaDeErrores>");
                    archivoTR.WriteLine("   <prefix>PO</prefix>");
                    archivoTR.WriteLine("   <readerConfig>" + rutaArchivo + @"Sistema\ticket\TR_SpringConfig.xml</readerConfig>");
                    archivoTR.WriteLine("   <cantidadDiasHistorico>365</cantidadDiasHistorico>");
                    archivoTR.WriteLine("   <entidadLiquidante>false</entidadLiquidante>");
                    archivoTR.WriteLine("   <capturaArchivos>true</capturaArchivos>");
                    archivoTR.WriteLine("</ConfiguracionArchivo>");
                    estadoAux = true;
                }
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
            finally
            {
                if (archivoTR != null)
                {
                    archivoTR.Close();
                }
            }
            return estadoAux;
        }

        public static void EnviaMensaje(Boolean mensajeAux)
        {
            Configuracion configuracion = new Configuracion(Directory.GetCurrentDirectory() + @"\config\cuitsConfig.xml");
            String flag = configuracion.rutaScripts + "reinicio.imatch";
            StreamWriter reinicio = null;
            try
            {
                if (mensajeAux)
                {
                    Console.Write("\tEnviando mensaje... ");
                    var values = new Dictionary<string, string>
                    {
                        { "token", configuracion.token },
                        { "channel", configuracion.canalSlack },
                        { "text", ":warning: Alta de Cuit, reinicio requerido..." }
                    };
                    HttpClient client = new HttpClient();
                    var content = new FormUrlEncodedContent(values);
                    var response = client.PostAsync("https://slack.com/api/chat.postMessage", content);
                    reinicio = new StreamWriter(flag);
                    reinicio.WriteLine("Reinicio Programado...");
                    Console.WriteLine("Ok.");
                    Console.WriteLine();
                }
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
            finally
            {
                if (reinicio != null)
                {
                    reinicio.Close();
                }
            }

        }

        public static void ReinicioServidor()
        {
            Configuracion configuracion = new Configuracion(Directory.GetCurrentDirectory() + @"\config\cuitsConfig.xml");
            String flag = configuracion.rutaScripts + "reinicio.imatch";
            try
            {
                if (File.Exists(flag))
                {
                    Console.Write("\tReiniciando servicio... ");
                    Int32 hora = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
                    if (hora > 0230 && hora < 0330)
                    {
                        DetenerServicio(configuracion.servicio);
                        IniciarServicio(configuracion.servicio);
                        File.Delete(flag);
                        Console.WriteLine("Ok.");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Pendiente.");
                        Console.WriteLine();
                    }
                }

            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
        }

        public static void DetenerServicio(String servicio)
        {
            ServiceController sc = new ServiceController(servicio);
            try
            {
                if (sc != null && sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                }
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                sc.Close();
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
        }

        public static void IniciarServicio(String servicio)
        {
            ServiceController sc = new ServiceController(servicio);
            try
            {
                if (sc != null && sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                }
                sc.WaitForStatus(ServiceControllerStatus.Running);
                sc.Close();
            }
            catch (Exception excepcion)
            {
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
            }
        }
    }
}
