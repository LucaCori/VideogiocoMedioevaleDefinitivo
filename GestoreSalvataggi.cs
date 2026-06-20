using System;
using System.IO;                 
using System.Collections.Generic;

namespace Program
{
    // 4. GESTIONE SALVATAGGI - REQUISITO: "Salvataggio/Resume partita"
    public static class GestoreSalvataggi
    {
        private static string percorsoFile = "salvataggio_gioco.txt";

        public static void SalvaPartita(Giocatore giocatore, List<Stanza> stanze)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(percorsoFile))
                {
                    //Salviamo il nome del giocatore
                    writer.WriteLine(giocatore.Nome);
                    //Salviamo il nome della stanza corrente
                    writer.WriteLine(giocatore.StanzaCorrente.Nome);
                    //Salviamo lo stato dell'interazione con l'NPC buono
                    writer.WriteLine($"Interazione con Personaggio Buono: {giocatore.HaInteragitoConBuono}");

                    //MODIFICA: Creiamo una lista dei nomi delle stanze già visitate 
                    List<string> nomiStanzeVisitate = new List<string>();
                    foreach(var stanza in stanze)
                    {
                        if (stanza.GiaVisitata)
                        {
                            nomiStanzeVisitate.Add(stanza.Nome);
                        }
                    }
                    //Uniamo i nomi delle stanze con una virgola e salviamo la riga
                    string rigaStanzeVisitate = string.Join(",", nomiStanzeVisitate);
                    writer.WriteLine($"Stanze Visitate: {rigaStanzeVisitate}");

                    //MODIFICA: Salviamo gli oggetti nell'inventario (Pila LIFO)
                    //Per mantenere l'ordine al caricamento, invertiamo temporaneamente la pila
                    //in modo che l'oggetto in fondo sia scritto per primo e quello in cima per ultimo
                    Stack<Oggetto> pilaInvertita = new Stack<Oggetto>(giocatore.InventarioPila);
                    List<string> nomiOggettiInventario = new List<string>();

                    foreach(var obj in pilaInvertita)
                    {
                        nomiOggettiInventario.Add(obj.Nome);
                    }
                    string rigaInventario = string.Join(",", nomiOggettiInventario);
                    writer.WriteLine($"Oggetti Inventario: {rigaInventario}");

                    Console.WriteLine("\n[Sistema] Partita salvata con successo!");
                    Logger.ScriviLog("Partita salvata manualmente con inventario e stato esplorazione.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\n[!] Errore durante il salvataggio: {ex.Message}");            
            }
        }

        public static bool CaricaPartita(List<Stanza> stanze, out string nome, out Stanza stanzaCorrente, out bool interagito, out List<string> oggettiDaCaricare)
        {
            nome = "Cavaliere";
            stanzaCorrente = stanze[0];
            interagito = false;
            oggettiDaCaricare = new List<string>();

            if(!File.Exists(percorsoFile)) return false;

            try
            {
                using(StreamReader reader = new StreamReader(percorsoFile))
                {
                    nome = reader.ReadLine() ?? "Cavaliere";
                    string? nomeStanza = reader.ReadLine();
                    string? rigaInterazione = reader.ReadLine();
                    string? rigaStanzeVisitate = reader.ReadLine();

                    // MODIFICA: Leggiamo la nuova riga dell'inventario
                    string? rigaInventario = reader.ReadLine();

                    Stanza? trovata = stanze.Find(s => s.Nome.Equals(nomeStanza, StringComparison.OrdinalIgnoreCase));
                    if(trovata != null) stanzaCorrente = trovata;

                    ///<summary>
                    /// MODIFICA LOGICA DI CARICAMENTO:
                    /// Se la riga non è nulla ed effettivamente contiene il nostro testo di riferimento
                    /// </summary>
                    if(rigaInterazione != null && rigaInterazione.StartsWith("Interazione con Personaggio Buono: "))
                    {
                        //Rimuoviamo il prefisso testuale per isolare solo "True" o "False"
                        string valoreBooleanoEstratto = rigaInterazione.Replace("Interazione con Personaggio Buono: ", "").Trim();

                        //Convertiamo la stringa pulita nel tipo bool
                        bool.TryParse(valoreBooleanoEstratto, out interagito);
                    }
                    //MODIFICA LOGICA DI CARICAMENTO STANZE:
                    //Resettiamo prima tutte le stanze a false (per sicurezza)
                    foreach(var s in stanze) s.GiaVisitata = false;

                    if(rigaStanzeVisitate != null && rigaStanzeVisitate.StartsWith("Stanze Visitate: "))
                    {
                        //Isoliamo la stringa contenente solo i nomi delle stanze separate da virgola
                        string elencoNomi = rigaStanzeVisitate.Replace("Stanze Visitate: ", "").Trim();
                        
                        if (!string.IsNullOrEmpty(elencoNomi))
                        {
                            // Spezziamo la stringa ricavando i singoli nomi delle stanze visitate
                            string[] nomiEstratti = elencoNomi.Split(',');
                            
                            foreach (string nomeStanzaEsplorata in nomiEstratti)
                            {
                                // Cerchiamo la stanza corrispondente nella mappa e impostiamo GiaVisitata = true
                                Stanza? stanzaEsplorata = stanze.Find(s => s.Nome.Equals(nomeStanzaEsplorata.Trim(), StringComparison.OrdinalIgnoreCase));
                                if (stanzaEsplorata != null)
                                {
                                    stanzaEsplorata.GiaVisitata = true;
                                }
                            }
                        }
                    }

                    //MODIFICA: Estraiamo i nomi degli oggetti da caricare nell'inventario
                    if(rigaInventario != null && rigaInventario.StartsWith("Oggetti Inventario: "))
                    {
                        string elencoOggetti = rigaInventario.Replace("Oggetti Inventario: ", "").Trim();
                        if (!string.IsNullOrEmpty(elencoOggetti))
                        {
                            string[] oggettiEstratti = elencoOggetti.Split(',');
                            foreach(string nomeObj in oggettiEstratti)
                            {
                                oggettiDaCaricare.Add(nomeObj.Trim());
                            }
                        }
                    }
                    else
                    {
                        //Caso di fallback se il file è vecchio o corrotto
                        interagito = false;
                    }

                    Logger.ScriviLog("Partita, cronologia esplorazione e inventario caricate con successo.");
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}