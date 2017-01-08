﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace UF2Practica1
{
	class MainClass
	{
		//Valors constants
		#region Constants
		const int nCaixeres = 3;

		#endregion
		/* Cua concurrent
		 	Dos mètodes bàsics: 
		 		Cua.Enqueue per afegir a la cua
		 		bool success = Cua.TryDequeue(out clientActual) per extreure de la cua i posar a clientActual
		*/

		public static ConcurrentQueue<Client> cua = new ConcurrentQueue<Client>();

		public static void Main(string[] args)
		{
			var clock = new Stopwatch();
			var threads = new List<Thread>();
			//Recordeu-vos que el fitxer CSV ha d'estar a la carpeta bin/debug de la solució
			const string fitxer = "CuaClients.csv";

			try
			{
				var reader = new StreamReader(File.OpenRead(@fitxer));


				//Carreguem la llista clients

				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(';');
					var tmp = new Client() { nom = values[0], carretCompra = Int32.Parse(values[1]) };
                    //Afegim el client a la cua
					cua.Enqueue(tmp);

				}

			}
			catch (Exception)
			{
				Console.WriteLine("Error accedint a l'arxiu");
				Console.ReadKey();
				Environment.Exit(0);
			}

			clock.Start();

                int i;

                // Instanciar les caixeres i afegir el thread creat a la llista de threads

                for (i = 0; i < nCaixeres; i++)
                {
                    /*Instanciem la caixera, generant la id, amb el contador del bucle(per exemple).
                     * Per a cada caixera que ens generin, creem un fil que s'encarregarà de fer la crida 
                     * per a gestionar les funcions que es criden
                     */
                    var caixera = new Caixera() { idCaixera = i };
                    //Mirem que la cua no estigui buida i fem la crida a pocessar cua

                        var fil = new Thread(() => caixera.ProcessarCua());
                        fil.Start();
                        threads.Add(fil);
                }            

            // Procediment per esperar que acabin tots els threads abans d'acabar
            foreach (Thread thread in threads)
                thread.Join();



            // Parem el rellotge i mostrem el temps que triga
            clock.Stop();
			double temps = clock.ElapsedMilliseconds / 1000;
			Console.Clear();
			Console.WriteLine("Temps total Task: " + temps + " segons");
			Console.ReadKey();
		}
	}
	#region ClassCaixera
	public class Caixera
	{
		public int idCaixera
		{
			get;
			set;
		}

		public void ProcessarCua()
		{

            // Llegirem la cua extreient l'element
            // cridem al mètode ProcesarCompra passant-li el client
            //(Mientras la cola no este vacia, cojo elemento y lo paso al metodo)

            while (!MainClass.cua.IsEmpty)
            {
                var clientActual = new Client();
                bool success = MainClass.cua.TryDequeue(out clientActual);

                if (success == true)
                {
                    ProcesarCompra(clientActual);
                }
            }     

        }


		private void ProcesarCompra(Client client)
		{

			Console.WriteLine("La caixera " + this.idCaixera + " comença amb el client " + client.nom + " que té " + client.carretCompra + " productes");

			for (int i = 0; i < client.carretCompra; i++)
			{
				this.ProcessaProducte();

			}

			Console.WriteLine(">>>>>> La caixera " + this.idCaixera + " ha acabat amb el client " + client.nom);
		}


		private void ProcessaProducte()
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
		}
	}


	#endregion

	#region ClassClient

	public class Client
	{
		public string nom
		{
			get;
			set;
		}

		public int carretCompra
		{
			get;
			set;
		}


	}

	#endregion
}
