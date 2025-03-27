import random
import json
import os

marcas_e_modelos = {
    "GM": ["Astra Sport", "Corsa", "Montana", "S10"],
    "Volkswagen": ["Gol", "Fusca", "Polo", "Passat"],
    "Fiat": ["Fiorino", "Uno", "Palio", "Toro"],
    "Ford": ["Fusca", "Focus", "Fiesta", "Maverick"],
    "Toyota": ["Corolla", "Hilux", "Yaris", "Etios"],
    "Honda": ["Civic", "Fit", "HR-V", "Accord"],
    "BMW": ["X6", "320i", "M3", "M4"],
    "Audi": ["Q3", "A3", "A4", "A5"],
    "Nissan": ["Sentra", "Altima", "Kicks", "Frontier"],
    "Hyundai": ["HB20", "Creta", "Santa Fe", "i30"]
}

fipe_range = [5000, 90000]

def gerar_veiculo():
    marca = random.choice(list(marcas_e_modelos.keys())) 
    modelo = random.choice(marcas_e_modelos[marca]) 
    ano_fabricacao = random.choice(range(1990, 2026))
      
    ano_modelo = ano_fabricacao if random.random() > 0.5 else ano_fabricacao + 1
   
    fipe = random.randint(fipe_range[0], fipe_range[1]) 

    return {
        "Modelo": modelo,
        "Marca": marca,
        "AnoFabricacao": ano_fabricacao,
        "AnoModelo": ano_modelo,
        "Fipe": fipe
    }

project_dir = os.path.dirname(os.path.abspath(__file__)) 
block_collection_dir = os.path.join(os.path.dirname(project_dir), 'BlockCollection', 'BlockCollection')

os.makedirs(block_collection_dir, exist_ok=True)

total_de_veiculos = 10
json_file_path = os.path.join(block_collection_dir, "veiculos.json")
veiculos = [gerar_veiculo() for _ in range(total_de_veiculos)]

with open(json_file_path, "w") as f:
    json.dump(veiculos, f, indent=4)

print(f"JSON de {total_de_veiculos} veículos gerado com sucesso em {json_file_path}!")
