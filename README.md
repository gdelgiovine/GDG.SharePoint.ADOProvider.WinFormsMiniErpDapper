# MiniERP Dapper — Entity Model, mapping e WinForms

I packages NUGET sono inclusi nel repository. Fatene buon uso e non rubateli.

Terza variante didattica del MiniERP: **Dapper + Model C#** su `GDG.SharePoint.ADOProvider`.
Progetto indipendente, WinForms `net48;net8.0-windows;net10.0-windows`. Le versioni originale e Classic
restano intatte.

## 1. Modello a oggetti

In `Models.cs` sono definiti quattro POCO: `Agente`, `Cliente`, `Ordine`,
`Allegato`. Ogni istanza rappresenta una riga della lista SharePoint; le
proprieta' hanno gli stessi nomi delle colonne **interne**, per consentire
il mapping automatico per nome di Dapper.

```csharp
public class Agente
{
    public long ID { get; set; }
    public string Title { get; set; } = "";
    public string AgentCode { get; set; } = "";
    public bool Active { get; set; } = true;
    public string Region { get; set; } = "";
}
```

```csharp
public List<Agente> CaricaAgenti()
{
    using (SharePointConnection cn = ApriConnessione())
    {
        return cn.Query<Agente>(
            "SELECT ID, Title, AgentCode, Active, Region " +
            "FROM [TutorialAgents] ORDER BY ID").ToList();
    }
}
```

**Non si usano `DataTable`, `DataRow`, `DataRowView` ne' `DataAdapter`**
nel MiniERP Dapper. Nella form principale:

```csharp
bsDati.DataSource = new BindingList<Agente>(dati.CaricaAgenti());
grdDati.DataSource = bsDati;

Agente selezionato = bsDati.Current as Agente;
```

L'editing produce un nuovo oggetto `Agente`/`Cliente`/`Ordine`;
il repository lo riceve direttamente: `dati.SalvaAgente(agente)`.
La scheda non modifica l'oggetto originale fino al salvataggio.

## 2. Mapping SharePoint e lookup

| Lista SharePoint | Model C# | Campi importanti |
| --- | --- | --- |
| `TutorialAgents` | `Agente` | `ID`, `Title`, `AgentCode`, `Active`, `Region` |
| `TutorialCustomers` | `Cliente` | `ID`, `Title`, `Code`, `Active`, `Country`, `AccountAgentId` |
| `TutorialOrders` | `Ordine` | `ID`, `Title`, `CustomerId`, `AgentId`, `CustomerCode`, `AgentCode`, `Status`, `Amount` |
| `$ATTACHMENTS(...)` | `Allegato` | `ItemId`, `FileName` |

Il provider restituisce i riferimenti lookup come campi ID
(`AccountAgentId`, `CustomerId`, `AgentId`). Per scrivere in SharePoint,
il comando SQL utilizza invece i nomi delle colonne lookup:
`AccountAgent`, `Customer`, `Agent`. Il repository usa
`DynamicParameters` tipizzati (`DbType.Int64`) per questo mapping.
Le `ComboBox` della scheda ricevono `List<Agente>` e `List<Cliente>`:
`DisplayMember = "Title"`, `ValueMember = "ID"`; le selezioni sono oggetti
C# e non `DataRowView`.

La corrispondenza nome colonna / proprieta' avviene in Dapper, non tramite
Entity Framework ne' tramite attributi di un ORM piu' complesso. Non
c'e' tracking automatico delle modifiche: `INSERT`, `UPDATE` e `DELETE`
rimangono istruzioni SQL parametrizzate esplicite.

## 3. CRUD e allegati

- `DatiSharePoint.CaricaAgenti/Clienti/Ordini()` restituiscono `List<T>`.
- `DatiSharePoint.SalvaAgente(Agente)`, `SalvaCliente(Cliente)`,
  `SalvaOrdine(Ordine)` ricevono un singolo Model.
- I metodi `EliminaAgente(Agente)`, `EliminaCliente(Cliente)` e
  `EliminaOrdine(Ordine)` proteggono i legami fra record.
- `CaricaAllegati()` restituisce `List<Allegato>`, collegata a
  `BindingList<Allegato>` per la griglia degli allegati.
- Upload, download, sostituzione, rinomina ed eliminazione sono implementati
  tramite la pseudo-tabella provider `$ATTACHMENTS`. Il contenuto binario si
  legge solo durante il download. Dimensione massima upload demo: **20 MiB**.
- `CustomerCode` e `AgentCode` negli ordini sono mantenuti insieme
  agli ID, in coerenza con la fixture del Tutorial.

Il MiniERP non crea e non azzera le liste. Preparare la fixture del Tutorial
**esclusivamente su un sito dimostrativo**, mai su dati di produzione.

## 4. Autenticazione e architettura

```text
MainForm / EditForm / AttachmentsForm
              |
     Agente, Cliente, Ordine, Allegato
              |
        DatiSharePoint.cs
              |
          Dapper 2.1.89
              |
      SharePointConnection
              |
    GDG.SharePoint.ADOProvider
              |
         REST SharePoint
```

Dapper riduce il codice di mapping, **non** parla direttamente a SharePoint.
I componenti di connessione e i form necessari sono inclusi localmente nel
progetto; non sono richiesti sorgenti del WinForms Tutorial. Per SharePoint
Online il flusso predefinito apre WebView2 per il login Microsoft 365/MFA e usa
i cookie SharePoint in memoria (`BrowserCookies`): non richiede Tenant ID,
Client ID o registrazione di un'app. È disponibile anche il flusso MSAL
facoltativo con app public-client configurata.
Per SharePoint Server SE sono disponibili credenziali Windows esplicite o
l'identità Windows corrente. I profili sono salvati separatamente in
`%LOCALAPPDATA%\GDG.SharePoint.ADOProvider\WinFormsMiniErpDapper`;
password, cookie e token non vengono salvati in `Connections.xml`.

## 5. Avvio e test

Aprire `GDG.SharePoint.ADOProvider.sln` in Visual Studio 2022/2026
e impostare `GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper` come progetto
di avvio. Selezionare `net48` o `net8.0-windows`. Quindi usare
**Connessioni...** e **Collega**.

```powershell
dotnet build .\samples\GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper\GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper.csproj -c Release -f net8.0-windows
dotnet .\samples\GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper\bin\Release\net8.0-windows\GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper.dll --self-test
```

`--self-test` verifica offline il mapping nominale Dapper dei POCO,
i tipi nullable dei lookup, il `BindingList<Model>`, le pseudo-tabelle
degli allegati, i nomi file e la presenza dell'assembly Dapper.
Non prova la deserializzazione di un reader SharePoint reale.

E' necessario un ulteriore collaudo **manuale** su SharePoint Online e/o SE
con login MFA, CRUD, lookup e allegati, oltre all'apertura dei form nel
Designer di Visual Studio 2026. Le operazioni CRUD restano sincrone, per
semplicita' didattica; richieste SharePoint lente possono bloccare la UI.
