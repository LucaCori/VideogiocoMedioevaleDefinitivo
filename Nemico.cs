using System;

namespace Program
{
    /// <summary>
    /// Rappresenta un personaggio ostile (Cattivo).
    /// </summary>
    public class Nemico : PersonaggioBase
    {
        public bool GiaAffrontato {get; set;}
        public Nemico(string nome, string descrizione) : base(nome, descrizione)
        {
            IsBuono = false;
            GiaAffrontato = false; //Di default inizia come non affrontato
        }

        ///<summary>
        /// Sovrascrittura del metodo di interazione per il nemico
        /// </summary>
        public override void Interagisci(Giocatore giocatore)
        {
            base.Interagisci(giocatore);
            Console.WriteLine($"\"{Descrizione}\"");

            // REQUISITO LIFO: Controlliamo se il giocatore ha la Spada ed è in CIMA alla pila (la sta stringendo in mano)
            bool haSpadaInMano = giocatore.InventarioPila.Count > 0 && 
                                 giocatore.InventarioPila.Peek().Nome.Equals("Spada", StringComparison.OrdinalIgnoreCase);

            if (!GiaAffrontato)
            {
                if (haSpadaInMano)
                {
                    Console.WriteLine($"[{Nome} ringhia e indietreggia]: 'Maledetto... non avvicinarti con quella Spada!'");
                    Logger.ScriviLog($"Giocatore ha spaventato il nemico {Nome} con la spada pronta in cima alla pila.");
                }
                else
                {
                    Console.WriteLine($"[{Nome} ti ringhia contro]: 'Sciocco cavaliere! Non uscirai vivo da questo castello e ti ruberò tutto!'");
                    Logger.ScriviLog($"Giocatore ha subito le minacce del nemico {Nome}");
                }
                
            }
            else
            {
                if (haSpadaInMano)
                {
                    Console.WriteLine($"[{Nome} ti osserva da lontano ringhiando]: 'Gnaff... tieni lontana quella lama!'");
                }
                else
                {
                    Console.WriteLine($"[{Nome} ti osserva da lontano]: 'Maledetto... non avvicinarti con quella Spada!");
                }
                Logger.ScriviLog($"Incontro ripetuto con il nemico {Nome}");
            }
            
        }
    }
}