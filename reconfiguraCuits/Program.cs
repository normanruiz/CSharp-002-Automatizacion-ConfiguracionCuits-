using System;
using System.Collections.Generic;
using System.Threading;

namespace reconfiguraCuits
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine();
                Console.WriteLine(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("   Verificando configuracion...");
                Console.WriteLine(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine();

                // Declaro array vacio que contendra el listado de Cuits a verificar.
                List<String> listadoCuits = new List<string>();

                // Leer el directorio en busca de cuits y lo almaceno en una arreglo.
                listadoCuits = Procesos.LoteDeCarga();

                // Ejecuto el lote de proceso en busca de posibles altas, retorno verdadero o falso
                // segun corresponda
                Boolean mensaje;

                mensaje = Procesos.LoteDeProceso(listadoCuits);

                // Evalue el resultado del proceso, en caso de existir un alta se emite un alerta.
                Procesos.EnviaMensaje(mensaje);

                // Reinicio el servidor en caso de ser nesesario.
                Procesos.ReinicioServidor();

                Console.WriteLine();
                Console.WriteLine(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("   Proceso finalizado.");
                Console.WriteLine(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine();

                Thread.Sleep(6000);

            }
            catch (Exception excepcion)
            {
                Console.WriteLine();
                Console.WriteLine("\tAlgo salio mal !!!");
                Console.WriteLine();
                Console.WriteLine("\t" + excepcion.Message);
                Console.WriteLine();
                Thread.Sleep(6000);
            }

        }
    }
}
