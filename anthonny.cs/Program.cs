using System;
using System.Collections.Generic;
using System.IO;

namespace RegistroGastos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string opcion;
            List<Gasto> gastos = new List<Gasto>();
            int siguienteId = 1;

            Cargar(gastos, ref siguienteId);

            do
            {
                opcion = LeerOpcionMenu();

                switch (opcion)
                {
                    case "1":
                        Agregar(gastos, ref siguienteId);
                        break;

                    case "2":
                        Listar(gastos);
                        break;

                    case "3":
                        BuscarCategoria(gastos);
                        break;

                    case "4":
                        Guardar(gastos);
                        Console.WriteLine("Saliendo...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida, intenta de nuevo.");
                        break;
                }

            }
            while (opcion != "4");
        }

        private static string LeerOpcionMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== REGISTRO DE GASTOS ===");
            Console.WriteLine("1. Agregar gasto");
            Console.WriteLine("2. Listar gastos");
            Console.WriteLine("3. Buscar por categoría");
            Console.WriteLine("4. Salir");
            Console.Write("Elige una opción: ");

            return Console.ReadLine();
        }

        private static void Agregar(List<Gasto> gastos, ref int id)
        {
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine().Trim();

            Console.Write("Monto: ");
            bool montoOk = decimal.TryParse(
                Console.ReadLine(),
                out decimal monto);

            Console.Write("Categoría: ");
            string categoria = Console.ReadLine().Trim();

            if (!montoOk || monto <= 0 ||
                string.IsNullOrWhiteSpace(descripcion) ||
                string.IsNullOrWhiteSpace(categoria))
            {
                Console.WriteLine("Datos inválidos.");
                return;
            }
            else
            {
                gastos.Add(new Gasto
                {
                    Id = id,
                    Descripcion = descripcion,
                    Monto = monto,
                    Categoria = categoria
                });

                id++;

                Console.WriteLine("Gasto agregado.");
            }
        }

        private static void Listar(List<Gasto> gastos)
        {
            if (gastos.Count == 0)
            {
                Console.WriteLine("No hay gastos registrados.");
                return;
            }

            decimal totalGeneral = 0;

            Console.WriteLine(
                $"{"Id",-3} {"Descripción",-20} {"Monto",10} {"Categoría",-15}");

            foreach (Gasto g in gastos)
            {
                Console.WriteLine(g);
                totalGeneral += g.Monto;
            }

            Console.WriteLine($"TOTAL GASTADO: Q {totalGeneral:N2}");
        }

        private static void BuscarCategoria(List<Gasto> gastos)
        {
            Console.Write("Categoría a buscar: ");

            string texto = Console.ReadLine().Trim().ToUpper();

            bool encontrado = false;

            foreach (Gasto g in gastos)
            {
                if (g.Categoria.ToUpper().Contains(texto))
                {
                    Console.WriteLine(g);
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("Sin coincidencias.");
            }
        }

        private static void Guardar(List<Gasto> gastos)
        {
            using (StreamWriter escritor = new StreamWriter("gastos.csv"))
            {
                foreach (Gasto g in gastos)
                {
                    escritor.WriteLine(
                        $"{g.Id};{g.Descripcion};{g.Monto};{g.Categoria}");
                }
            }

            Console.WriteLine("Gastos guardados en gastos.csv.");
        }

        private static void Cargar(List<Gasto> gastos, ref int siguienteId)
        {
            if (!File.Exists("gastos.csv"))
            {
                return;
            }

            string[] lineas = File.ReadAllLines("gastos.csv");

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(';');

                if (datos.Length == 4)
                {
                    Gasto g = new Gasto();

                    g.Id = int.Parse(datos[0]);
                    g.Descripcion = datos[1];
                    g.Monto = decimal.Parse(datos[2]);
                    g.Categoria = datos[3];

                    gastos.Add(g);

                    if (g.Id >= siguienteId)
                    {
                        siguienteId = g.Id + 1;
                    }
                }
            }

            if (gastos.Count > 0)
            {
                Console.WriteLine(
                    $"Cargados {gastos.Count} gastos desde gastos.csv.");
            }
        }
    }
}
