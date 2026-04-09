using GrowsmartAPI.Models;
using System.Collections.Generic;

namespace GrowsmartAPI.Data;

public static class SeedData
{
    public static List<PlantSpecies> GetInitialSpecies()
    {
        return new List<PlantSpecies>
        {
            // AGRICULTURAL CROPS
            new PlantSpecies { 
                CommonName = "Rice", 
                ScientificName = "Oryza sativa", 
                Category = "Agricultural",
                Description = "A primary cereal grain and a staple food for a large part of the world's human population.",
                CareTips = "• Water: Constant flooding or saturated soil\n• Sun: Full sun (6-8 hours)\n• Fertilizer: High nitrogen during vegetative phase",
                HorticultureSuggestions = "Marigold (pest repellent), Ornamental Grasses, Lotus"
            },
            new PlantSpecies { 
                CommonName = "Wheat", 
                ScientificName = "Triticum aestivum", 
                Category = "Agricultural",
                Description = "A cereal grain which is a worldwide staple food.",
                CareTips = "• Water: Moderate, critical during flowering\n• Sun: Bright direct light\n• Soil: Well-drained loamy soil",
                HorticultureSuggestions = "Cornflowers, Poppies, Lavender (field borders)"
            },
            new PlantSpecies { 
                CommonName = "Tomato", 
                ScientificName = "Solanum lycopersicum", 
                Category = "Agricultural",
                Description = "Versatile red berry used as a vegetable in cooking.",
                CareTips = "• Sun: 8 hours direct sun\n• Water: Deep watering at the base\n• Support: Stakes or cages required",
                HorticultureSuggestions = "Basil (flavour companion), Marigolds, Nasturtiums"
            },
            new PlantSpecies { 
                CommonName = "Potato", 
                ScientificName = "Solanum tuberosum", 
                Category = "Agricultural",
                Description = "Starchy tuberous crop from the perennial nightshade family.",
                CareTips = "• Soil: Loose, acidic soil\n• Hilling: Cover stems with soil as they grow\n• Water: Even moisture, avoid waterlogging",
                HorticultureSuggestions = "Petunias, Sweet Alyssum, Pansies"
            },
            new PlantSpecies { 
                CommonName = "Mango", 
                ScientificName = "Mangifera indica", 
                Category = "Agricultural",
                Description = "Tropical stone fruit known as the 'king of fruits'.",
                CareTips = "• Climate: Warm, frost-free\n• Pruning: Annual pruning after harvest\n• Water: Regular for young trees, drought-tolerant when mature",
                HorticultureSuggestions = "Bougainvillea, Hibiscus, Plumeria (understory or borders)"
            },
            new PlantSpecies { 
                CommonName = "Cotton", 
                ScientificName = "Gossypium", 
                Category = "Agricultural",
                Description = "Soft, fluffy staple fiber that grows in a boll.",
                CareTips = "• Climate: Long frost-free period\n• Water: High water requirement during boll development\n• Soil: Heavy, nutrient-rich",
                HorticultureSuggestions = "Morning Glory, Zinnia, Sunflower"
            },

            // HORTICULTURE / ORNAMENTAL
            new PlantSpecies { 
                CommonName = "Rose", 
                ScientificName = "Rosa", 
                Category = "Horticulture",
                Description = "A woody perennial flowering plant known for its beauty and fragrance.",
                CareTips = "• Pruning: Late winter pruning is essential\n• Water: Deeply twice a week\n• Sun: Morning sun is best",
                HorticultureSuggestions = "Lavender, Salvia, Baby's Breath"
            },
            new PlantSpecies { 
                CommonName = "Marigold", 
                ScientificName = "Tagetes", 
                Category = "Horticulture",
                Description = "Cheerful orange/yellow flowers that act as natural pest deterrents.",
                CareTips = "• Sun: Full sun\n• Water: Allow soil to dry between waterings\n• Deadheading: Remove faded flowers to encourage blooming",
                HorticultureSuggestions = "Zinnias, Salvia, Ornamental Peppers"
            },
            new PlantSpecies { 
                CommonName = "Snake Plant", 
                ScientificName = "Dracaena trifasciata", 
                Category = "Horticulture",
                Description = "Hardy indoor plant that improves air quality.",
                CareTips = "• Water: Only when soil is completely dry\n• Light: Tolerates low light to bright indirect\n• Temperature: Keep above 10°C",
                HorticultureSuggestions = "Succulents, ZZ Plant, Pothos"
            },
            new PlantSpecies { 
                CommonName = "Aloe Vera", 
                ScientificName = "Aloe vera", 
                Category = "Horticulture",
                Description = "Succulent plant with medicinal gel in its leaves.",
                CareTips = "• Soil: Well-draining cactus mix\n• Water: Drench and dry method\n• Potting: Terracotta pots preferred",
                HorticultureSuggestions = "Echeveria, Jade Plant, Haworthia"
            }
        };
    }

    public static List<CommonDisease> GetInitialDiseases()
    {
        return new List<CommonDisease>
        {
            new CommonDisease {
                Name = "Rice Blast",
                ScientificName = "Magnaporthe oryzae",
                Description = "A fungal disease that causes lesions on leaves, nodes, and panicles.",
                Symptoms = "Spindle-shaped spots with grey centers on leaves; diamond-shaped lesions.",
                TreatmentPlan = "1. Use resistant varieties\n2. Avoid excessive nitrogen\n3. Apply fungicides like Tricyclazole",
                AffectedPlants = "Rice, Wheat"
            },
            new CommonDisease {
                Name = "Late Blight",
                ScientificName = "Phytophthora infestans",
                Description = "Serious disease affecting potatoes and tomatoes, responsible for the Irish Potato Famine.",
                Symptoms = "Dark, water-soaked spots on leaf edges; white fungal growth on undersides in humid weather.",
                TreatmentPlan = "1. Improve air circulation\n2. Remove infected debris\n3. Use copper-based fungicides",
                AffectedPlants = "Tomato, Potato"
            },
            new CommonDisease {
                Name = "Powdery Mildew",
                ScientificName = "Erysiphales",
                Description = "Common fungal disease appearing as white flour-like powder on plant surfaces.",
                Symptoms = "White circular patches on leaves and stems; leaves may turn yellow and drop.",
                TreatmentPlan = "1. Spray Neem oil\n2. Mix baking soda with water and spray\n3. Improve sunlight exposure",
                AffectedPlants = "Rose, Grapes, Cucurbits, Mango"
            },
            new CommonDisease {
                Name = "Black Spot",
                ScientificName = "Diplocarpon rosae",
                Description = "One of the most common diseases for roses, especially in wet climates.",
                Symptoms = "Circular black spots on leaves surrounded by yellow halos; premature leaf drop.",
                TreatmentPlan = "1. Prune infected stems\n2. Water only at the base\n3. Regular sulfur or fungicide sprays",
                AffectedPlants = "Rose"
            }
        };
    }
}
