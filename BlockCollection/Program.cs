using BlockCollection;
using OfficeOpenXml;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Newtonsoft.Json;


const string apiKey = "YOUR_API_KEY";
await Run();

static async Task Run()
{
    string basePath = AppDomain.CurrentDomain.BaseDirectory;
    string projectPath = Directory.GetParent(basePath).Parent.Parent.Parent.FullName;
    string jsonFilePath = Path.Combine(projectPath, "veiculos.json");

    string json = File.ReadAllText(jsonFilePath);
    List<Veiculo> automoveis = JsonConvert.DeserializeObject<List<Veiculo>>(json);

    Stopwatch swSequencial = Stopwatch.StartNew();
    await ConsultarModeloCarroSequencial(automoveis);
    swSequencial.Stop();

    BlockingCollection<Veiculo> bcAutomoveis = new BlockingCollection<Veiculo>();
    Stopwatch swBlockCollection = Stopwatch.StartNew();
    Task producer = Producer(bcAutomoveis, automoveis);
    Task consumer = Task.Run(() => Consumer(bcAutomoveis));

    await Task.WhenAll(producer, consumer);
    swBlockCollection.Stop();

    Console.WriteLine($"\nTempo SEM BlockingCollection: {swSequencial.ElapsedMilliseconds} ms \n");
    Console.WriteLine($"\nTempo Com BlockingCollection: {swBlockCollection.ElapsedMilliseconds} ms \n");
}

static async Task Producer(BlockingCollection<Veiculo> bcAutomoveis, List<Veiculo> automoveis)
{
    foreach (var automovel in automoveis)
        bcAutomoveis.Add(automovel);

    bcAutomoveis.CompleteAdding();
}

static async Task Consumer(BlockingCollection<Veiculo> bcAutomoveis)
{
    foreach (var automovel in bcAutomoveis.GetConsumingEnumerable())
    {
        string searchUrlTemplate = $"https://serpapi.com/search?q={Uri.EscapeDataString(automovel.Modelo)}+carro&engine=google&api_key={apiKey}";
        dynamic jsonResponse = null;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                string resultado = await client.GetStringAsync(searchUrlTemplate);
                jsonResponse = JsonConvert.DeserializeObject<dynamic>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar o modelo {automovel.Modelo}: {ex.Message}");
                continue;
            }
        }

        if (jsonResponse?.organic_results != null && jsonResponse.organic_results.Count > 0)
        {
            string titulo = jsonResponse.organic_results[0]?.title ?? "Sem título";
            string link = jsonResponse.organic_results[0]?.link ?? "Sem link";

            Console.WriteLine($"BlockingCollection - Modelo: {automovel.Modelo} - Resultado encontrado: {titulo} - Link: {link}");           
        }
    }
}

static async Task ConsultarModeloCarroSequencial(List<Veiculo> automoveis)
{  
    foreach (var automovel in automoveis)
    {
        string searchUrlTemplate = $"https://serpapi.com/search?q={Uri.EscapeDataString(automovel.Modelo)}+carro&engine=google&api_key={apiKey}";
        dynamic jsonResponse = null;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                string resultado = await client.GetStringAsync(searchUrlTemplate);
                jsonResponse = JsonConvert.DeserializeObject<dynamic>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar o modelo {automovel.Modelo}: {ex.Message}");
                continue;
            }
        }

        if (jsonResponse?.organic_results != null && jsonResponse.organic_results.Count > 0)
        {
            string titulo = jsonResponse.organic_results[0]?.title ?? "Sem título";
            string link = jsonResponse.organic_results[0]?.link ?? "Sem link";

            Console.WriteLine($"Sequencial - Modelo: {automovel.Modelo} - Resultado encontrado: {titulo} - Link: {link}");
        }
    }
}


static Dictionary<string, decimal> CalcularFipePorMarcaSequencial(List<Veiculo> automoveis)
{
    Dictionary<string, decimal> fipePorMarca = new Dictionary<string, decimal>();

    foreach (var automovel in automoveis)
    {
        if (fipePorMarca.ContainsKey(automovel.Marca))
            fipePorMarca[automovel.Marca] += automovel.Fipe;
        else
            fipePorMarca[automovel.Marca] = automovel.Fipe;
    }

    return fipePorMarca;
}

static async Task<Dictionary<string, decimal>> ConsumerRetornandoValores(BlockingCollection<Veiculo> bcAutomoveis)
{
    Dictionary<string, decimal> fipePorMarca = new Dictionary<string, decimal>();

    foreach (var automovel in bcAutomoveis.GetConsumingEnumerable())
    {
        if (fipePorMarca.ContainsKey(automovel.Marca))
            fipePorMarca[automovel.Marca] += automovel.Fipe;
        else
            fipePorMarca[automovel.Marca] = automovel.Fipe;
    }

    return fipePorMarca;
}


static void ExibirResultado(IDictionary<string, decimal> resultado)
{
    foreach (var item in resultado)
    {
        Console.WriteLine($"Marca: {item.Key}, Soma da FIPE: {item.Value}");
    }
}