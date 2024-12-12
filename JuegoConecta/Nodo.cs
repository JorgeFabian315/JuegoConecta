using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuegoConecta
{
    public class Nodo
    {
        public int[,] Tablero { get; set; }
        public int JugadorActual { get; set; }
        public List<Nodo> Hijos { get; set; }
        public int Valor { get; set; }
        public bool EsTerminal { get; set; }
        public int Filas { get; set; } = 6;
        public int Columnas { get; set; } = 7;
        

        public Nodo(int[,] tablero = null, int jugadorActual = 1)
        {
            Tablero = tablero ?? new int[Filas, Columnas];
            JugadorActual = jugadorActual;
            Hijos = new List<Nodo>();
            Valor = 0;
            EsTerminal = VerificarEstadoTerminal();
        }


        public void GenerarSucesores()
        {
            if (EsTerminal)
                return;

            for (int col = 0; col < Columnas; col++)
            {
                if (Tablero[0, col] == 0)
                {
                    int[,] nuevoTablero = (int[,])Tablero.Clone();
                    HacerMovimiento(nuevoTablero, col, JugadorActual);
                    Hijos.Add(new Nodo(nuevoTablero, JugadorActual == 1 ? 2 : 1));
                }
            }
        }

        private void HacerMovimiento(int[,] tablero, int columna, int jugador)
        {
            for (int fila = Filas - 1; fila >= 0; fila--)
            {
                if (tablero[fila, columna] == 0)
                {
                    tablero[fila, columna] = jugador;
                    break;
                }
            }
        }
        private bool VerificarEstadoTerminal()
        {
            return HayGanador() || TableroLleno();
        }
        private bool HayGanador()
        {
            // Verifica filas, columnas y diagonales para encontrar 5 fichas consecutivas.
            return VerificarLineas(5);
        }

        private bool TableroLleno()
        {
            for (int col = 0; col < Columnas; col++)
            {
                if (Tablero[0, col] == 0)
                    return false;
            }
            return true;
        }

       
        private bool VerificarLineas(int cantidad)
        {
           //filas
            for (int fila = 0; fila < Filas; fila++)
            {
                for (int col = 0; col <= Columnas - cantidad; col++)
                {
                    int jugador = Tablero[fila, col];
                    if (jugador != 0 && EsLineaConsecutiva(fila, col, 0, 1, cantidad, jugador))
                        return true;
                }
            }

            // columnas
            for (int col = 0; col < Columnas; col++)
            {
                for (int fila = 0; fila <= Filas - cantidad; fila++)
                {
                    int jugador = Tablero[fila, col];
                    if (jugador != 0 && EsLineaConsecutiva(fila, col, 1, 0, cantidad, jugador))
                        return true;
                }
            }

            // Verificar diagonales hacia abajo-derecha
            for (int fila = 0; fila <= Filas - cantidad; fila++)
            {
                for (int col = 0; col <= Columnas - cantidad; col++)
                {
                    int jugador = Tablero[fila, col];
                    if (jugador != 0 && EsLineaConsecutiva(fila, col, 1, 1, cantidad, jugador))
                        return true;
                }
            }

            // Verificar diagonales hacia abajo-izquierda
            for (int fila = 0; fila <= Filas - cantidad; fila++)
            {
                for (int col = cantidad - 1; col < Columnas; col++)
                {
                    int jugador = Tablero[fila, col];
                    if (jugador != 0 && EsLineaConsecutiva(fila, col, 1, -1, cantidad, jugador))
                        return true;
                }
            }

            return false;
        }
        
        private bool EsLineaConsecutiva(int fila, int columna, int deltaFila, int deltaColumna, int cantidad, int jugador)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (Tablero[fila + i * deltaFila, columna + i * deltaColumna] != jugador)
                    return false;
            }
            return true;
        }

        #region Minimax
        /// <summary>
        /// Implementa el algoritmo Minimax.
        /// </summary>
        /// <param name="profundidad">La profundidad máxima de búsqueda.</param>
        /// <param name="maximizando">Indica si estamos maximizando o minimizando.</param>
        /// <returns>El valor del nodo.</returns>
        public int Minimax(int profundidadBusqueda, bool maximizando)
        {
            if (EsTerminal || profundidadBusqueda == 0)
            {
                return CalcularHeuristica();
            }

            if (maximizando)
            {
                int mejorValor = int.MinValue;
                GenerarSucesores();

                foreach (var hijo in Hijos)
                {
                    int valor = hijo.Minimax(profundidadBusqueda - 1, false);
                    mejorValor = Math.Max(mejorValor, valor);
                }

                Valor = mejorValor;
                return mejorValor;
            }
            else
            {
                int mejorValor = int.MaxValue;
                GenerarSucesores();

                foreach (var hijo in Hijos)
                {
                    int valor = hijo.Minimax(profundidadBusqueda - 1, true);
                    mejorValor = Math.Min(mejorValor, valor);
                }

                Valor = mejorValor;
                return mejorValor;
            }
        }

        private int CalcularHeuristica()
        {
            if (HayGanador())
            {
                return JugadorActual == 2 ? 1000 : -1000;
            }

            int puntaje = 0;

            
            puntaje += ContarLineasParciales(4, JugadorActual) * 100; 
            puntaje += ContarLineasParciales(3, JugadorActual) * 10;  
            puntaje += ContarLineasParciales(2, JugadorActual) * 1;  
          
            int oponente = JugadorActual == 1 ? 2 : 1;
            puntaje -= ContarLineasParciales(4, oponente) * 100;
            puntaje -= ContarLineasParciales(3, oponente) * 10;
            puntaje -= ContarLineasParciales(2, oponente) * 1;

            return puntaje;
        }

        private int ContarLineasParciales(int cantidad, int jugador)
        {
            int total = 0;

            // Filas
            for (int fila = 0; fila < Filas; fila++)
            {
                for (int col = 0; col <= Columnas - cantidad; col++)
                {
                    if (EsLineaParcial(fila, col, 0, 1, cantidad, jugador))
                    {
                        total++;
                    }
                }
            }

            // Columnas
            for (int col = 0; col < Columnas; col++)
            {
                for (int fila = 0; fila <= Filas - cantidad; fila++)
                {
                    if (EsLineaParcial(fila, col, 1, 0, cantidad, jugador))
                    {
                        total++;
                    }
                }
            }

            // Diagonales
            for (int fila = 0; fila <= Filas - cantidad; fila++)
            {
                for (int col = 0; col <= Columnas - cantidad; col++)
                {
                    if (EsLineaParcial(fila, col, 1, 1, cantidad, jugador))
                    {
                        total++;
                    }
                }
            }

            
            for (int fila = 0; fila <= Filas - cantidad; fila++)
            {
                for (int col = cantidad - 1; col < Columnas; col++)
                {
                    if (EsLineaParcial(fila, col, 1, -1, cantidad, jugador))
                    {
                        total++;
                    }
                }
            }
            return total;
        }

        private bool EsLineaParcial(int fila, int columna, int deltaFila, int deltaColumna, int cantidad, int jugador)
        {
            int fichasJugador = 0;
            int espaciosVacios = 0;

            for (int i = 0; i < cantidad; i++)
            {
                int filaActual = fila + i * deltaFila;
                int columnaActual = columna + i * deltaColumna;

                if (filaActual < 0 || filaActual >= Filas || columnaActual < 0 || columnaActual >= Columnas)
                    return false;

                if (Tablero[filaActual, columnaActual] == jugador)
                {
                    fichasJugador++;
                }
                else if (Tablero[filaActual, columnaActual] == 0)
                {
                    espaciosVacios++;
                }
                else
                {               
                    return false;
                }
            }
            // Es una línea parcial válida si contiene solo fichas del jugador y/o espacios vacíos.
            return fichasJugador > 0 && (fichasJugador + espaciosVacios == cantidad);
        }

        #endregion

        #region Maquina
        public Nodo ElegirMejorMovimiento()
        {
            GenerarSucesores();
            Nodo mejorMovimiento = null;
            int mejorValor = int.MinValue;

            foreach (var hijo in Hijos)
            {
                int valor = hijo.Minimax(5, false); // Profundidad máxima de 5.
                if (valor > mejorValor)
                {
                    mejorValor = valor;
                    mejorMovimiento = hijo;
                }
            }

            return mejorMovimiento;
        }
        #endregion
    }

}
