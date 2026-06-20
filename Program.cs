using System;
using System.Collections.Generic;

namespace Program
{
    // 5. PROGRAMMA PRINCIPALE
    class Program
    {
        //Teniamo una lista globale di tutte le stanze per gestire il teletrasporto casuale
        public static List<Stanza> tutteLeStanze = new List<Stanza>();
        public static Random random = new Random();
        static void Main(string[] args)
        {
            //REQUISITO: "Gestire un file di configurazione, cui leggere alcuni parametri a runtime"
            int pesoMassimoConfig = 12;
            string percorsoConfig = "config.txt";

            if (!File.Exists(percorsoConfig))
            {
                //Se non esiste, lo creiamo scrivendoci un parametro standard
                File.WriteAllText(percorsoConfig, "PESO_MASSIMO=12");
            }
            else
            {
                //Se esiste, lo leggiamo a runtime
                string[] righeConfig = File.ReadAllLines(percorsoConfig);
                foreach(string riga in righeConfig)
                {
                    if (riga.StartsWith("PESO_MASSIMO="))
                    {
                        int.TryParse(riga.Replace("PESO_MASSIMO=", ""), out pesoMassimoConfig);
                    }
                }
            }
            
            Logger.ScriviLog("--- Avvio di una nuova sessione di gioco ---");
            //REQUISITO: "Mostrare un messaggio introduttivo"
            Console.WriteLine("==========================================================");
            Console.WriteLine("      BENVENUTO NELL'ANNO DOMINI 1326: THE LOST CROWN     ");
            Console.WriteLine("==========================================================");
            Console.WriteLine("Il reame è nel caos. La corona è svanita e creature strane");
            Console.WriteLine("popolano i corridoi del castello. Trova il saggio, recupera");
            Console.WriteLine("la corona e riprenditi la Sala del Trono per vincere.");
            Console.WriteLine("==========================================================\n");

            //REQUISITO: "Il gioco ha diverse stanze (almeno4 4)" - Configurazione Mappa
            Stanza taverna = new Stanza ("Taverna del Drago Verde", "Un luogo caldo, fumoso e pieno di avventurieri.");
            Stanza cortile = new Stanza ("Cortile del Castello", "Un ampio spazio aperto circondato da alte mura di pietra.");
            Stanza armeria = new Stanza ("Armeria Reale", "Scaffali pieni di armi e armature impolverate.");
            Stanza prigione = new Stanza ("Prigioni Sotterranee", "Un luogo buio e freddo, si sente l'umidità sui muri.");
            //Nuova stanza per la condizione di vittoria
            Stanza salaTrono = new Stanza("Sala del Trono", "La maestosa sala del Re. Qui risiede il potere del regno.");
            //NUOVA STANZA: La cucina
            Stanza cucina = new Stanza("Cucina del Castello", "Un ambiente ampio con un grande focolare spento e tavoli pieni di farina.");

            //Popoliamo la lista globale delle stanze
            tutteLeStanze.Add(taverna);
            tutteLeStanze.Add(cortile);
            tutteLeStanze.Add(armeria);
            tutteLeStanze.Add(prigione);
            tutteLeStanze.Add(salaTrono);
            tutteLeStanze.Add(cucina);

            //Colleghiamo le stanze (N, S, W, E)
            taverna.ImpostaCollegamento("Nord", cortile);
            cortile.ImpostaCollegamento("Sud", taverna);
            cortile.ImpostaCollegamento("Est", armeria);
            cortile.ImpostaCollegamento("Ovest", prigione);
            cortile.ImpostaCollegamento("Nord", salaTrono); //La sala del trono è a Nord del cortile
            salaTrono.ImpostaCollegamento("Sud", cortile);
            armeria.ImpostaCollegamento("Ovest", cortile);
            prigione.ImpostaCollegamento("Est", cortile);
            taverna.ImpostaCollegamento("Sud", cucina);
            cucina.ImpostaCollegamento("Nord", taverna);

            //REQUISITO: "Ci sono oggetti in alcune stanze..." e Posizioniamo gli oggetti nelle stanze
            taverna.OggettiPresenti.Add(new Oggetto("Boccale", "Un vecchio boccale di legno sbeccato che profuma di sidro.", true, 2));
            armeria.OggettiPresenti.Add(new Oggetto("Spada", "Una solida spada da fante in acciaio delle Marche.", true, 5));
            cortile.OggettiPresenti.Add(new Oggetto("Cesoie", "Strumento usato per la potatura degli arbusti. Ha le lame ormai usurate", true, 3));
            prigione.OggettiPresenti.Add(new Oggetto("Stoffa Lacerata", "Sembra essere quel che rimane di un vecchio arazzo.", false, 0));
            cucina.OggettiPresenti.Add(new Oggetto("Mestolo Arrugginito", "In passato deve aver servito molte pietanze ai sudditi del regno", true, 1));
            cortile.OggettiPresenti.Add(new Oggetto("Piuma d'Oca", "Caduta a terra da un soffice cuscino lacerato.", false, 0));

            //REQUISITO: "Ci sono altri personaggi che si trovano in stanze casuali"
            PersonaggioBase saggio = new Alleato("Saggio", "Un vecchio mentore con una lunga barba bianca che conosce i segreti del regno.");
            PersonaggioBase goblin = new Nemico("Goblin", "Una creatura viscida e verde che borbotta parole incomprensibili.");
            //NUOVO PERSONAGGIO: Il Viandante fisso nel Cortile
            PersonaggioBase viandante = new PersonaggioGuida("Viandante", "Un viaggiatore incapucciato seduto su un muretto all'ombra.", "Ascoltami bene, Cavaliere... tra queste mura si aggirano strane creature pronte a rubare le tue ricchezze. Se trovi una Spada, tienila in cima al tuo inventario! Solo se la stringi come ultimo oggetto raccolto potrai sguainarla in tempo per difenderti!");

            //Lo assegniamo direttamente al Cortile del Castello prima del posizionamento degli altri
            cortile.PersonaggioPresente = viandante;

            ///<summary
            /// POSIZIONAMENTO SICURO DEGLI NPC (Senza sovrascritture)
            /// </summary>
            
            //1. Posizionamento del Saggio
            Stanza stanzaSaggio;
            do
            {
                int indiceCasuale = random.Next(0, tutteLeStanze.Count);
                stanzaSaggio = tutteLeStanze[indiceCasuale];
            }
            while(stanzaSaggio.Nome.Equals("Cortile del Castello", StringComparison.OrdinalIgnoreCase) || stanzaSaggio.PersonaggioPresente != null); //Se la stanza è già occupata, estrai di nuovo

            stanzaSaggio.PersonaggioPresente = saggio;
            Logger.ScriviLog($"Saggio posizionato nella stanza: {stanzaSaggio.Nome}");

            //2. Posizionamento sicuro del Goblin
            Stanza stanzaGoblin;
            do
            {
                int indiceCasuale = random.Next(0, tutteLeStanze.Count);
                stanzaGoblin = tutteLeStanze[indiceCasuale];
            }
            while(stanzaGoblin.Nome.Equals("Cortile del Castello", StringComparison.OrdinalIgnoreCase) || stanzaGoblin.PersonaggioPresente != null); //Continua a cercare finché non trovi una stanza libera

            stanzaGoblin.PersonaggioPresente = goblin;
            Logger.ScriviLog($"Goblin posizionato nella stanza: {stanzaGoblin.Nome}");

            //Posizionamento sicuro e randomico della CORONA (Escludendo Cortile e Sala del Trono)
            Stanza stanzaCorona;
            do
            {
                //Estraiamo un indice su tutte le stanze disponibili (da 0 a 4)
                int indiceCasuale = random.Next(0, tutteLeStanze.Count);
                stanzaCorona = tutteLeStanze[indiceCasuale];
            }
            //Il ciclo continua se la stanza estratta è il Cortile OPPURE la Sala del Trono
            while(stanzaCorona.Nome.Equals("Cortile del Castello", StringComparison.OrdinalIgnoreCase) || stanzaCorona.Nome.Equals("Sala del Trono", StringComparison.OrdinalIgnoreCase));

            //Aggiungiamo fisicamente la Corona nella lista degli oggetti della stanza estratta
            stanzaCorona.OggettiPresenti.Add(new Oggetto("Corona", "La mitica corona d'oro tempestata di rubini luccicanti.", true, 3));
            Logger.ScriviLog($"Corona posizionata randomicamente nella stanza: {stanzaCorona.Nome}");

            //Posizionamento sicuro e randomico del TALISMANO (Escludendo il Cortile del Castello)
            Stanza stanzaTalismano;
            do
            {
                int indiceCasuale = random.Next(0, tutteLeStanze.Count);
                stanzaTalismano = tutteLeStanze[indiceCasuale];
            }
            while(stanzaTalismano.Nome.Equals("Cortile del Castello", StringComparison.OrdinalIgnoreCase));

            //Aggiungiamo il Talismano nella stanza estratta (isPrendibile: true peso: 4kg)
            stanzaTalismano.OggettiPresenti.Add(new Oggetto("Talismano", "Un antico amuleto di pietra che vibra di energia magica instabile.", true, 4));
            Logger.ScriviLog($"Talismano posizionato randomicamente nella stanza: {stanzaTalismano.Nome}");

            //REQUISITO: "Salvataggio/Resume partita"
            string nomeGiocatore = "Cavaliere";
            Stanza stanzaIniziale = cortile;
            bool giaInteragitoConBuono = false;

            //Nuova lista di supporto per contenere temporaneramente i nomi degli oggetti salvati
            List<string> oggettiSalvati = new List<string>();

            Console.WriteLine("Vuoi caricare l'ultimo salvataggio (S/N): ");
            string? rispostaCarica = Console.ReadLine();
            if(rispostaCarica != null && rispostaCarica.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                //MODIFICA: Passiamo anche il parametro out 'oggettiSalvati'
                GestoreSalvataggi.CaricaPartita(tutteLeStanze, out nomeGiocatore, out stanzaIniziale, out giaInteragitoConBuono, out oggettiSalvati);
            }
            else
            {
                Console.WriteLine("Inserisci il nome del tuo cavaliere: ");
                string? inputNome = Console.ReadLine();
                nomeGiocatore = string.IsNullOrWhiteSpace(inputNome) ? "Cavaliere" : inputNome;
            }

            //Inizializziamo il giocatore posizionandolo nella Taverna (Peso Massimo = 10 kg)
            Giocatore giocatore = new Giocatore(nomeGiocatore, stanzaIniziale, pesoMassimoConfig);
            giocatore.HaInteragitoConBuono = giaInteragitoConBuono;

            //MODIFICA LOGICA DI RICOSTRUZIONE INVENTARIO:
            //Cerchiamo gli oggetti salvati ovunque si trovino (nelle stanze) e facciamo il Push dell'inventario
            foreach(string nomeOggettoSalvato in oggettiSalvati)
            {
                Oggetto? oggettoTrovato = null;

                //Cerchiamo l'ogetto in tutte le stanze del gioco per capire dove si torva attualmente
                foreach(var stanza in tutteLeStanze)
                {
                    oggettoTrovato = stanza.OggettiPresenti.Find(o => o.Nome.Equals(nomeOggettoSalvato, StringComparison.OrdinalIgnoreCase));
                    if(oggettoTrovato != null)
                    {
                        //Trovato! Lo rimuoviamo dalla stanza terrena e lo mettiamo nell'inventario del giocatore
                        stanza.OggettiPresenti.Remove(oggettoTrovato);
                        giocatore.InventarioPila.Push(oggettoTrovato);
                        break;
                    }
                }
            }

            //LOOP DI GIOCO SPERIMENTALE
            bool inEsecuzione = true;
            while (inEsecuzione)
            {
                //REQUISITO: "Il giocatore può vincere. Ci deve essere una situazione riconsciuta come la fine del gioco..."
                //Condizione di vittoria: portare la Corona nella Sala del Trono
                //REQUISITO: "E' necessario interagire con il personaggio buono per risolvere il gioco"
                //UPDATE: Modifica condizione di vittoria
                bool coronaUsataNelTrono = salaTrono.OggettiPresenti.Exists(o => o.Nome.Equals("Corona", StringComparison.OrdinalIgnoreCase));
                if(giocatore.StanzaCorrente == salaTrono && coronaUsataNelTrono)
                {
                    //STEP 1: Controllo interazione con il personaggio buono (Saggio)
                    if (giocatore.HaInteragitoConBuono)
                    {
                        Console.WriteLine("\n==================================================");
                        Console.WriteLine($"COMPLIMENTI {giocatore.Nome.ToUpper()}! HAI VINTO IL GIOCO!");
                        Console.WriteLine("RISPETTATI TUTTI GLI STEP DI VITTORIA:");
                        Console.WriteLine("1. Hai incontrato e ottenuto il favore del Vecchio Saggio.");
                        Console.WriteLine("2. Hai recuperato la mitica Corona rispettando la logica LIFO.");
                        Console.WriteLine("3. Sei giunto nella Sala del Trono e hai USATO la Corona per incoronarti!");
                        Console.WriteLine("==================================================");
                        Logger.ScriviLog("Partita terminata con la vittoria del giocatore.");
                    break;
                    }
                    else
                    {
                        //Se l'utente ha usato la corona ma non ha mai parlato con il saggio
                        Console.WriteLine("\n[!] Ti trovi sul trono con la corona, ma il popolo non ti riconosce... Ti manca il consiglio e la benedizione del Vecchio Saggio per poter reclamare il regno!");
                        //Riprendiamo la corona da terra e la rimettiamo in cima all'inventario per permettergli di continuare
                        Oggetto? coronaDaTerra = salaTrono.OggettiPresenti.Find(o => o.Nome.Equals("Corona", StringComparison.OrdinalIgnoreCase));
                        if(coronaDaTerra != null)
                        {
                            salaTrono.OggettiPresenti.Remove(coronaDaTerra);
                            giocatore.InventarioPila.Push(coronaDaTerra);
                        }
                    }
                }
                Console.WriteLine("\n------------------------------------------------");
                Console.WriteLine($"Stanza Attuale: {giocatore.StanzaCorrente.Nome}");

                //REQUISITO: "mostrata in automatico alla prima visita"
                if(!giocatore.StanzaCorrente.GiaVisitata)
                {
                    Console.WriteLine($"[PRIMA VISITA] {giocatore.StanzaCorrente.Descrizione}");
                    
                    // --- NUOVA MODIFICA: Mostra le uscite automaticamente alla prima visita ---
                    string[] direzioniMappa = {"NORD", "SUD", "EST", "OVEST"};
                    bool porteTrovate = false;

                    foreach(string dir in direzioniMappa)
                    {
                        Stanza? adiacente = giocatore.StanzaCorrente.OttieniStanzaAdiacente(dir);
                        if(adiacente != null)
                        {
                            //Formattiamo la stringa (es. NORD -> Nord)
                            string dirFormattata = char.ToUpper(dir[0]) + dir.Substring(1).ToLower();
                            Console.WriteLine($"- A {dirFormattata} c'è: {adiacente.Nome}");
                            porteTrovate = true;
                        }
                    }
                    if(!porteTrovate) Console.WriteLine("- Nessuna uscita visibile.");
                    giocatore.StanzaCorrente.GiaVisitata = true;
                }
                else
                {
                    Console.WriteLine("(Sei già stato qui)");
                }

                if(giocatore.StanzaCorrente.PersonaggioPresente != null)
                {
                    Console.WriteLine($"[NPC] In questa stanza c'è {giocatore.StanzaCorrente.PersonaggioPresente.Nome}.");
                }

                if(giocatore.StanzaCorrente.OggettiPresenti.Count > 0)
                {
                    Console.WriteLine("Oggetti per terra: ");
                    foreach(var obj in giocatore.StanzaCorrente.OggettiPresenti) Console.WriteLine($"[{obj.Nome}] ");
                    Console.WriteLine();
                }

                //Input comandi utente
                Console.Write("\nCosa vuoi fare?: ");
                string? inputComando = Console.ReadLine();
                string comandoInterno = inputComando != null ? inputComando.Trim() : "";

                //MODIFICA: Rimuoviamo automaticamente le parentesi quadre se l'utente le inserisce per errore
                comandoInterno = comandoInterno.Replace("[", "").Replace("]", "");

                string[] partiComando = comandoInterno.Split(' ', 2);
                string azione = partiComando[0].ToUpper();

                if(azione == "Q")
                {
                    inEsecuzione = false;
                    Console.WriteLine("Grazie per aver giocato!");
                }
                //REQUISITO: "Helper"
                else if(azione == "HELP")
                {
                    MostraHelper();
                }
                else if(azione == "NORD" || azione == "SUD" || azione == "EST" || azione == "OVEST")
                {
                    //Salviamo la stanza in cui si trova il giocatore prima di muoversi 
                    Stanza stanzaPrecedente = giocatore.StanzaCorrente;

                    //Eseguiamo il movimento
                    giocatore.Muovi(azione);

                    //Controlliamo se il giocatore ha SPOSTATO effettivamente la sua posizione (il movimento è riuscito)
                    if(giocatore.StanzaCorrente != stanzaPrecedente)
                    {
                        //SE LA STANZA PRECEDENTE AVEVA UN NEMICO, LO RESETTIAMO!
                        //In questo modo, se il giocatore ritorna in futuro, il nemico attaccherà
                        if(stanzaPrecedente.PersonaggioPresente is Nemico nemicoPrecedente)
                        {
                            nemicoPrecedente.GiaAffrontato = false;
                        }
                        //IL GIOCATORE E' ENTRATO IN UNA NUOVA STANZA: Verifichiamo SUBITO se c'è un nemico ad attenderlo
                        if(giocatore.StanzaCorrente.PersonaggioPresente != null && giocatore.StanzaCorrente.PersonaggioPresente is Nemico nemico)
                        {
                            Console.WriteLine($"\n[!] AGGHICCIANTE SORPRESA! In questa stanza c'è {nemico.Nome}");
                            nemico.Interagisci(giocatore);

                            if (!nemico.GiaAffrontato)
                            {
                                if(giocatore.InventarioPila.Count > 0)
                                {
                                    bool haSpadaInCima = giocatore.InventarioPila.Count > 0 && giocatore.InventarioPila.Peek().Nome.Equals("Spada", StringComparison.OrdinalIgnoreCase);
                                    bool attaccoAnnullato = false;

                                    if (haSpadaInCima)
                                    {
                                        Console.WriteLine($"\n[!] ATTENZIONE: Stringi la '{giocatore.InventarioPila.Peek().Nome}' in cima al tuo inventario!");
                                        Console.WriteLine("Vuoi usarla per difenderti dal furto? (S/N): ");
                                        string? rispostaDifesa = Console.ReadLine();

                                        if(rispostaDifesa != null && rispostaDifesa.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Console.WriteLine($"\n[Combattimento] Sguaini rapidamente la Spada! {nemico.Nome} si spaventa e indietreggia. Attacco annullato!");
                                            Logger.ScriviLog($"Il giocatore si è difeso dall'attacco di {nemico.Nome} usando la Spada.");
                                            attaccoAnnullato = true;
                                        }
                                    }
                                    if (!attaccoAnnullato)
                                    {
                                        Oggetto oggettoRubato = giocatore.InventarioPila.Pop();
                                        Console.WriteLine($"\n[!] {nemico.Nome} ti ha sorpeso alle spalle e ti ha rubato l'oggetto in cima all'inventario: {oggettoRubato.Nome}!");

                                        List<Stanza> stanzeDisponibiliPerFurto = tutteLeStanze.FindAll(s => s != giocatore.StanzaCorrente);
                                        Stanza stanzaDestinazioneOggetto = stanzeDisponibiliPerFurto[random.Next(stanzeDisponibiliPerFurto.Count)];
                                        stanzaDestinazioneOggetto.OggettiPresenti.Add(oggettoRubato);

                                        Console.WriteLine($"[!] Senti il rumore dell'oggetto che cade da qualche parte nel castello...");
                                        Logger.ScriviLog($"Il nemico {nemico.Nome} ha rubato {oggettoRubato.Nome} e lo ha nascosto in {stanzaDestinazioneOggetto.Nome}.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\n[{nemico.Nome} ti fruga nelle tasche]: 'Non hai niente da rubare, straccione!'");
                                }
                                nemico.GiaAffrontato = true;
                            }
                        }
                    }
                }
                else if(azione == "PRENDI" && partiComando.Length > 1)
                {
                    giocatore.RaccogliOggetto(partiComando[1]);
                }
                else if(azione == "POSA" && partiComando.Length > 1)
                {
                    giocatore.UsaOPosaOggetto(partiComando[1], posaNellaStanza: true);
                }
                else if(azione == "USA" && partiComando.Length > 1)
                {
                    giocatore.UsaOPosaOggetto(partiComando[1], posaNellaStanza: false);
                }
                //REQUISITO: "Comandi per interagire con i personaggi"
                else if(azione == "PARLA")
                {
                    //Se l'utente ha scritto solo "PARLA" senza specificare il nome
                    if(partiComando.Length < 2)
                    {
                        Console.WriteLine("\n[!] Sintassi errata. Devi specificare con chi parlare (es: PARLA SAGGIO).");
                        continue; //Salta il resto del ciclo e chiede un nuovo comando
                    }

                    string nomeTarget = partiComando[1];

                    if(giocatore.StanzaCorrente.PersonaggioPresente != null)
                    {
                        // Controlliamo se il nome digitato corrisponde all'NPC nella stanza
                        if(giocatore.StanzaCorrente.PersonaggioPresente.Nome.Equals(nomeTarget, StringComparison.OrdinalIgnoreCase))
                        {
                            // Se corrisponde, facciamo partire l'interazione!
                            giocatore.StanzaCorrente.PersonaggioPresente.Interagisci(giocatore);
                        }
                        else
                        {
                            // Se inserisci un nome diverso da chi è presente
                            Console.WriteLine($"\n[!] Non c'è nessun '{nomeTarget}' qui con cui parlare.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n[!] Non c'è nessuno con cui parlare in questa stanza.");
                    }
                }
                //REQUISITO: "Salvataggio/Resume partita"
                else if(azione == "SALVA")
                {
                    GestoreSalvataggi.SalvaPartita(giocatore, tutteLeStanze);
                }
                else if(azione == "INVENTARIO")
                {
                    giocatore.MostraInventario();
                }
                else if(azione == "GUARDATI" && partiComando.Length > 1 && partiComando[1].ToUpper() == "INTORNO")
                {
                    Console.WriteLine("\n--- TI GUARDI INTORNO ---");
                    Console.WriteLine($"Ti trovi in: {giocatore.StanzaCorrente.Nome}");
                    Console.WriteLine("Le vie d'uscita visibili sono:");

                    //Elenco delle 4 direzioni possibili nel gioco
                    string[] direzioniDaControllare = {"NORD", "SUD", "EST", "OVEST"};
                    bool usciteTrovate = false;

                    foreach(string dir in direzioniDaControllare)
                    {
                        //Sfruttiamo il metodo esistente della classe Stanza per vedere cosa c'è
                        Stanza? stanzaAdiacente = giocatore.StanzaCorrente.OttieniStanzaAdiacente(dir);

                        if(stanzaAdiacente != null)
                        {
                            //Formattiamo la direzione per l'output (es. NORD -> Nord)
                            string dirFormattata = char.ToUpper(dir[0]) + dir.Substring(1).ToLower();

                            Console.WriteLine($"- A {dirFormattata} vedi: {stanzaAdiacente.Nome}");
                            usciteTrovate = true;
                        }
                    }

                    if (!usciteTrovate)
                    {
                        Console.WriteLine("[!] Strano... sembri completamente intrappolato tra quattro mura.");
                    }
                    Console.WriteLine("-------------------------");
                }
                //REQUISITO: "Implementare un teletrasporto che conduce il giocatore in una stanza casuale"
                else
                {
                    Console.WriteLine("[!] Comando sconosciuto. Digita 'HELP'.");
                }
            }
        }
        //REQUISITO: "Helper"
        static void MostraHelper()
        {
            Console.WriteLine("\n--- MANUALE DEI COMANDI ---");
            Console.WriteLine("NORD, SUD, EST, OVEST  -> Spostati nelle direzioni cardinali");
            Console.WriteLine("GUARDATI INTORNO    -> Elenca le stanze collegate e le loro direzioni");
            Console.WriteLine("PRENDI nome_oggetto -> Raccogli l'oggetto esaminandolo (es: PRENDI SPADA)");
            Console.WriteLine("POSA nome_oggetto -> Lascia un oggetto nella stanza (es: POSA SPADA)");
            Console.WriteLine("USA nome_oggetto -> Utilizza un oggetto presente nella pila");
            Console.WriteLine("PARLA nome_npc   -> Interagisci con il personaggio nella stanza (es: PARLA SAGGIO)");
            Console.WriteLine("INVENTARIO       -> Mostra la pila dei tuoi oggetti (LIFO)");
            Console.WriteLine("SALVA            -> Salva lo stato di gioco su file");
            Console.WriteLine("Q                -> Esci dal gioco");
            Console.WriteLine("---------------------------");
        }
    }
}
