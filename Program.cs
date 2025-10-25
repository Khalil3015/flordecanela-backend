// ===================== USING =====================
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

// ================= TOP-LEVEL STATEMENTS =================

var builder = WebApplication.CreateBuilder(args);

// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://flordecanela.netlify.app", "https://flordecanela.online")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ⚙️ Agregar controladores y autorización (ambos son necesarios)
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Compresión de respuesta
builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.Providers.Add<BrotliCompressionProvider>();
    o.Providers.Add<GzipCompressionProvider>();
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(o =>
{
    o.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(o =>
{
    o.Level = CompressionLevel.Fastest;
});

// ✅ Construir la app una sola vez
var app = builder.Build();

// Fuerza HTTPS/HSTS en prod
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// 🔒 Cabeceras de seguridad comunes
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=()";
    await next();
});

// Archivos estáticos con control de caché
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.Context.Request.Path.Value ?? "";
        if (path.EndsWith("index.html", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        }
        else
        {
            ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
        }
    }
});

// Middleware ordenado
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowFrontend"); // 👈 CORS debe ir antes de MapControllers
app.MapControllers(); // ✅ ahora funcionará correctamente
// ---------- CATEGORÍAS ----------
var categories = new List<Category> {
    new("salados","Bocaditos salados"),
    new("sandwiches","Sanduchitos, triples y dúos"),
    new("dulces","Bocaditos dulces"),
    new("shots-dulces","Shots dulces (2 oz)"),
    new("cakes","Cake’s"),
    new("bebidas","Bebidas")
};

// ---------- ÍTEMS ----------
var items = new List<Item> {
    // — SALADOS (25/50/100 y algunos 24/48/100)
    new Item("mini-cheese-bacon-burger","Mini Cheese & Bacon Burger","salados", new[]{"burger"}, Tiers.T25(89,172,340),null,"/img/catalogo/9.jpg"),
    new Item("petipan-pollo-apio","Petipán de pollo (con/sin apio)","salados", new[]{"petipán"}, Tiers.T25(68,132,260),null,"/img/catalogo/1.jpg"),
    new Item("petipan-pollo-durazno","Petipán de pollo con durazno","salados", new[]{"petipán"}, Tiers.T25(68,132,260),null,"/img/catalogo/2.jpg"),
    new Item("mini-croissant-pollo","Mini croissant de pollo","salados", new[]{"croissant"}, Tiers.T25(65,125,245),null,"/img/catalogo/5.jpg"),
    new Item("mini-croissant-hjq","Mini croissant de huevo, jamón y queso","salados", new[]{"croissant"}, Tiers.T25(65,125,245),null,"/img/catalogo/6.jpg"),
    new Item("mini-cheese-burger","Mini Cheese Burger","salados", new[]{"burger"}, Tiers.T25(85,165,325),null,"/img/catalogo/10.jpg"),
    new Item("mini-frances-mix","Mini francés (jamón, queso, lechuga, tomate)","salados", new[]{"sandwich"}, Tiers.T25(65,125,245),null,"/img/catalogo/3.jpg"),
    new Item("mini-capresse","Mini capresse","salados", null, Tiers.T25(70,135,265),null,"/img/catalogo/4.jpg"),
    new Item("mini-frankfurter","Mini frankfurter","salados", null, Tiers.T25(70,135,265),null,"/img/catalogo/12.jpg"),
    new Item("mini-choripanes","Mini choripanes (con salsa BBQ)","salados", new[]{"criollo"}, Tiers.T25(70,135,265),null,"/img/catalogo/8.jpg"),
    new Item("mini-pizzas","Mini pizzas","salados", new[]{"horneado"}, Tiers.T25(75,145,285),null,"/img/catalogo/17.jpg"),
    new Item("alitas-bbq","Alitas BBQ","salados", null, Tiers.T25(75,145,285),null,"/img/catalogo/22.jpg"),
    new Item("alitas-buffalo","Alitas Buffalo picantes","salados", null, Tiers.T25(65,125,245),null,"/img/catalogo/21.jpg"),
    new Item("spring-rolls-veggies","Spring rolls de col, zanahoria y pimiento","salados", new[]{"veg"}, Tiers.T25(60,115,225),null,"/img/catalogo/13.jpg"),
    new Item("spring-rolls-pollo","Spring rolls de pollo, col, zanahoria y pimiento","salados", null, Tiers.T25(75,145,285),null,"/img/catalogo/14.jpg"),
    new Item("bolitas-queso","Bolitas crocantes de queso","salados", null, Tiers.T25(60,115,225),null,"/img/catalogo/18.jpg"),
    new Item("mini-chicken-fingers","Mini chicken fingers + crema especial","salados", null, Tiers.T25(75,145,285),null,"/img/catalogo/15.jpg"),
    new Item("alitas-bouchet","Alitas estilo Bouchet","salados", null, Tiers.T25(75,145,285),null,"/img/catalogo/23.jpg"),
    new Item("bacon-ranch-cheese-balls","Mini Bacon Ranch Cheese Balls (queso Edam + crema)","salados", null, Tiers.T25(75,145,285),null,"/img/catalogo/16.jpg"),
    new Item("alitas-acevichadas","Alitas acevichadas","salados", null, Tiers.T25(65,125,245),null,"/img/catalogo/24.jpg"),

    new Item("shot-veggie","Shot veggie (2 oz)","salados", new[]{"incluye cucharita"}, Tiers.T24(80,155,305),"Incluyen cucharitas.","/img/catalogo/46.jpg"),
    new Item("shot-aji-gallina","Shot de ají de gallina (2 oz)","salados", new[]{"incluye cucharita"}, Tiers.T24(80,155,305),"Incluyen cucharitas.","/img/catalogo/46.jpg"),
    new Item("shot-causa-pollo","Shot de causitas de pollo (2 oz)","salados", new[]{"incluye cucharita"}, Tiers.T24(80,155,305),null,"/img/catalogo/45.jpg"),
    new Item("shot-atun-acevichado","Shot de atún acevichado (2 oz)","salados", new[]{"incluye cucharita"}, Tiers.T24(85,165,325),null,"/img/catalogo/46.jpg"),

    new Item("petipan-especial-pecanas","Petipán especial de pollo con pecanas","salados", new[]{"petipán"}, Tiers.T24_96(70,135,265),null,"/img/catalogo/25.jpg"),
    new Item("empanaditas-pollo","Empanaditas de pollo","salados", null, Tiers.T25(55,105,205),null,"/img/catalogo/33.jpg"),
    new Item("empanaditas-carne","Empanaditas de carne","salados", null, Tiers.T25(55,105,205),null,"/img/catalogo/34.jpg"),
    new Item("mini-croissant-queso-pimiento","Mini croissant con queso crema y pimiento rostizado","salados", null, Tiers.T25(60,115,225),null,"/img/catalogo/27.jpg"),
    new Item("rolls-tocino-parmesano","Rolls de tocino, parmesano y orégano","salados", new[]{"horneado"}, Tiers.T12_24_48_96(35,65,125,245),null,"/img/catalogo/30.jpg"),
    new Item("rolls-pepperoni","Rolls de pepperoni","salados", new[]{"horneado"}, Tiers.T12_24_48_96(35,65,125,245),null,"/img/catalogo/31.jpg"),
    new Item("rolls-hojaldrados-tocino","Rolls hojaldrados con tocino, parmesano y orégano","salados", new[]{"horneado"}, Tiers.T12_24_48_96(35,65,125,245),null,"/img/catalogo/32.jpg"),
    new Item("mini-papitas-queso-pepperoni","Mini papitas rellenas con queso y pepperoni","salados", null, Tiers.T25(70,135,265),null,"/img/catalogo/29.jpg"),
    new Item("mini-papitas-carne","Mini papitas rellenas con carne","salados", null, Tiers.T25(70,135,265),null,"/img/catalogo/35.jpg"),
    new Item("mini-papitas-queso","Mini papitas rellenas con queso","salados", null, Tiers.T25(60,115,225),null,"/img/catalogo/36.jpg"),
    new Item("petipan-negro-pecanas-pasas","Petipán negro de pollo con pecanas y pasas","salados", new[]{"petipán"}, Tiers.T25(70,135,265),null,"/img/catalogo/26.jpg"),
    new Item("mini-panino-serrano-gouda","Mini panino de jamón serrano y queso gouda","salados", null, Tiers.T25(75,145,285),null,"/img/catalogo/28.jpg"),

    new Item("tequenos-queso","Tequeños de queso + crema de palta","salados", null, Tiers.T25(60,115,225),null,"/img/catalogo/41.jpg"),
    new Item("tequenos-jyq","Tequeños de jamón y queso + crema de palta","salados", null, Tiers.T25(65,125,245),null,"/img/catalogo/42.jpg"),
    new Item("tequenos-lomo","Tequeños de lomo saltado + crema de ají","salados", null, Tiers.T25(95,185,365),null,"/img/catalogo/43.jpg"),
    new Item("mini-chicharrones","Mini chicharrones","salados", new[]{"criollo"}, Tiers.T25(90,175,345),null,"/img/catalogo/37.jpg"),
    new Item("mini-pan-asado","Mini pan con asado (opcional cebolla caramelizada)","salados", new[]{"criollo"}, Tiers.T25(65,125,245),null,"/img/catalogo/39.jpg"),
    new Item("mini-pan-pavo","Mini pan con pavo","salados", null, Tiers.T25(65,125,245),null,"/img/catalogo/38.jpg"),
    new Item("mini-huachana","Mini pan con salchicha huachana","salados", null, Tiers.T25(60,115,225),null,"/img/catalogo/48.jpg"),
    new Item("mini-butifarras","Mini butifarras","salados", new[]{"criollo"}, Tiers.T25(65,125,245),null,"/img/catalogo/40.jpg"),
    new Item("enrrolladitos-hot-dog","Enrrolladitos de hot dog","salados", null, Tiers.T25(60,115,225),null,"/img/catalogo/44.jpg"),
};

// — SANDUCHITOS / TRIPLES / DÚOS
items.AddRange(new Item[] {
    new Item("triple-palta-tomate-queso","Triple: palta, tomate y queso","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/53.jpg"),
    new Item("triple-jamon-pollo-huevo","Triple: jamón, pollo y huevo","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/49.jpg"),
    new Item("triple-queso-peperoni-tomate","Triple: queso, peperoni y tomate","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/57.jpg"),
    new Item("triple-pollo-espinaca-quesocrema","Triple: pollo y espinaca con queso crema","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/54.jpg"),
    new Item("triple-pollo-jamon-durazno","Triple: pollo, jamón y durazno","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/58.jpg"),
    new Item("triple-palta-tomate-huevo","Triple: palta, tomate y huevo","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/50.jpg"),
    new Item("triple-pollo-jamon-queso","Triple: pollo, jamón y queso","sandwiches", null, Tiers.T24_96(65,125,245),null,"/img/catalogo/51.jpg"),
    new Item("duo-jamon-queso","Dúo: jamón y queso","sandwiches", null, Tiers.T24_96(58,112,220),null,"/img/catalogo/56.jpg"),
    new Item("duo-jamon-aceituna-huevo","Dúo: jamón, aceituna y huevo","sandwiches", null, Tiers.T24_96(58,112,220),null,"/img/catalogo/55.jpg"),
    new Item("duo-pollo-durazno","Dúo: pollo y durazno","sandwiches", null, Tiers.T24_96(58,112,220),null,"/img/catalogo/52.jpg"),
});

// — SHOTS DULCES y CHEESECAKES
items.AddRange(new Item[] {
    new Item("mini-cheesecake-oreo","Mini cheesecakes de Oreo","shots-dulces", new[]{"incluye cucharita"}, Tiers.T24(80,155,305),null,"/img/catalogo/59.jpg"),
    new Item("mini-cheesecake-limon","Mini cheesecakes de limón","shots-dulces", null, Tiers.T24(85,165,325),null,"/img/catalogo/63.jpg"),
    new Item("mini-cheesecake-pina-colada","Mini cheesecakes de piña colada","shots-dulces", null, Tiers.T24(80,155,305),null,"/img/catalogo/67.jpg"),
    new Item("mini-cheesecake-mango","Mini cheesecakes de mango","shots-dulces", null, Tiers.T24(85,165,325),null,"/img/catalogo/64.jpg"),
    new Item("mini-cheesecake-fresa","Mini cheesecakes de fresa","shots-dulces", null, Tiers.T24(80,155,305),null,"/img/catalogo/60.jpg"),
    new Item("mini-cheesecake-arandanos","Mini cheesecakes de arándanos","shots-dulces", null, Tiers.T24(80,155,305),null,"/img/catalogo/68.jpg"),
    new Item("mini-cheesecake-maracuya","Mini cheesecakes de maracuyá","shots-dulces", null, Tiers.T24(85,165,325),null,"/img/catalogo/61.jpg"),
    new Item("mini-cheesecake-maracumango","Mini cheesecakes de maracumango","shots-dulces", null, Tiers.T24(80,155,305),null,"/img/catalogo/69.jpg"),
    new Item("mini-cheesecake-durazno","Mini cheesecakes de durazno","shots-dulces", null, Tiers.T24(80,155,305),null,"/img/catalogo/65.jpg"),
    new Item("mini-tiramisu","Mini tiramisú","shots-dulces", null, Tiers.T24(85,165,325),null,"/img/catalogo/62.jpg"),
    new Item("mini-chocotorta-arg","Mini chocotorta argentina","shots-dulces", null, Tiers.T24(70,135,265),null,"/img/catalogo/66.jpg"),
    new Item("mini-suspiro-limeno","Mini suspiro limeño","shots-dulces", new[]{"incluye cucharita"}, Tiers.T24(75,145,285),null,"/img/catalogo/70.jpg"),
    new Item("mini-parfait-yogurt","Mini parfait de yogurt, granola y frutas","shots-dulces", new[]{"incluye cucharita"}, Tiers.T24(75,145,285),null,"/img/catalogo/71.jpg"),
    new Item("shot-frutal","Shot frutal (frutas de estación)","shots-dulces", new[]{"incluye cucharita"}, Tiers.T24(70,135,265),null,"/img/catalogo/72.jpg"),
});

// — DULCES
items.AddRange(new Item[] {
    new Item("orejitas","Orejitas","dulces", null, Tiers.T25(60,115,225),null,"/img/catalogo/77.jpg"),
    new Item("mini-alfajores","Mini alfajores","dulces", null, Tiers.T25(50,95,185),null,"/img/catalogo/81.jpg"),
    new Item("mini-alfajores-choco","Mini alfajores cubiertos de chocolate","dulces", null, Tiers.T25(58,112,220),null,"/img/catalogo/82.jpg"),
    new Item("trufas-chocolate","Trufas de chocolate","dulces", null, Tiers.T25(50,95,185),null,"/img/catalogo/75.jpg"),
    new Item("trufas-oreo","Trufas de Oreo","dulces", null, Tiers.T25(58,112,220),null,"/img/catalogo/73.jpg"),
    new Item("trufas-mani","Trufas de chocolate con maní","dulces", null, Tiers.T25(58,112,220),null,"/img/catalogo/74.jpg"),
    new Item("mini-brownies","Mini brownies chocolatodos","dulces", null, Tiers.T24(90,175,345),null,"/img/catalogo/78.jpg"),
    new Item("galletas-cookies-cream","Mini galletas de cookies and cream","dulces", null, Tiers.T25(58,112,220),null,"/img/catalogo/79.jpg"),
    new Item("galletas-chocochips","Mini galletas de choco chips","dulces", null, Tiers.T25(58,112,220),null,"/img/catalogo/80.jpg"),
    new Item("conitos-manjar","Conitos de manjar","dulces", null, Tiers.T25(60,115,225),null,"/img/catalogo/76.jpg"),
    new Item("piononitos-manjar","Piononitos de manjar","dulces", null, Tiers.T25(60,115,225),null,"/img/catalogo/83.jpg"),
    new Item("arroz-leche-5oz","Mini arroz con leche (5 oz)","dulces", new[]{"incluye cucharita"}, Tiers.T24(85,165,325),null,"/img/catalogo/85.jpg"),
    new Item("arroz-manjar-5oz","Mini arroz con manjar (5 oz)","dulces", new[]{"incluye cucharita"}, Tiers.T24(90,175,345),null,"/img/catalogo/86.jpg"),
    new Item("arroz-coco-pasas-5oz","Mini arroz con coco y pasas (5 oz)","dulces", new[]{"incluye cucharita"}, Tiers.T24(90,175,345),null,"/img/catalogo/87.jpg"),
    new Item("combinado-5oz","Mini combinado arroz y mazamorra (5 oz)","dulces", new[]{"incluye cucharita"}, Tiers.T24(90,175,345),null,"/img/catalogo/88.jpg"),
    new Item("mazamorra-morada-5oz","Mini mazamorra morada (5 oz)","dulces", new[]{"incluye cucharita"}, Tiers.T24(90,175,345),null,"/img/catalogo/84.jpg"),
});

// — CAKES (precio por unidad)
items.AddRange(new Item[] {
    new Item("cake-naranja","Cake de naranja","cakes", null, Tiers.Cake(50), "Tamaño 21×11×7 cm · 6–8 porciones","/img/catalogo/89.jpg"),
    new Item("cake-maracuya","Cake de maracuyá","cakes", null, Tiers.Cake(60), "Tamaño 21×11×7 cm · 6–8 porciones","/img/catalogo/90.jpg"),
    new Item("cake-blueberry-limon","Cake de blueberry y limón","cakes", null, Tiers.Cake(60), "Tamaño 21×11×7 cm · 6–8 porciones","/img/catalogo/91.jpg"),
    new Item("cake-platano-chocolate","Cake de plátano y chocolate","cakes", null, Tiers.Cake(60), "Tamaño 21×11×7 cm · 6–8 porciones","/img/catalogo/92.jpg"),
});

// — BEBIDAS (sin precios → “consultar”)
items.AddRange(new Item[] {
    new Item("cafe-pasado","Café pasado","bebidas", null, Array.Empty<PriceTier>(), "3 L (10–12 pax) · 5 L (18–20 pax) — Precio a consultar","/img/catalogo/93.jpg"),
    new Item("chocolate-leche","Chocolate con leche","bebidas", null, Array.Empty<PriceTier>(), "3 L · 5 L — Precio a consultar","/img/catalogo/93.jpg"),
    new Item("jugos-naturales","Jugos naturales (fresa, piña, papaya, maracuyá)","bebidas", null, Array.Empty<PriceTier>(), "3 L / 5 L — Precio a consultar","/img/catalogo/94.jpg"),
    new Item("chicha-morada","Chicha morada","bebidas", null, Array.Empty<PriceTier>(), "5 L (18–20 pax) — Precio a consultar","/img/catalogo/94.jpg"),
    new Item("aguas-saborizadas","Aguas saborizadas","bebidas", null, Array.Empty<PriceTier>(), "Jarra 1.5 L (6–8 pax) · 5 L — Precio a consultar","/img/catalogo/95.jpg"),
    new Item("gaseosas-3l","Gaseosas (variadas)","bebidas", null, Array.Empty<PriceTier>(), "Botella 3 L — Precio a consultar","/img/catalogo/95.jpg"),
});

// ---------- PACKS ----------
var packsVariados = new List<Pack> {
    new("pv-1","Pack Variado #1","variado",125,"15–20 personas",325, new[]{
        "Petipán de pollo (con/sin apio)",
        "Mini Cheese Burger",
        "Mini croissant mixto",
        "Mini alfajores",
        "Trufas de chocolate"
    }),
    new("pv-2","Pack Variado #2","variado",198,"25–30 personas",496, new[]{
        "Petipán de pollo con durazno",
        "Mini butifarras",
        "Enrrolladitos de hot dog",
        "Dúo: jamón y queso"
    }),
    new("pv-3","Pack Variado #3","variado",321,"35–40 personas",819, new[]{
        "Spring rolls de pollo, col, zanahoria y pimiento",
        "Conitos de manjar",
        "Orejitas",
        "Mini cheesecakes de Oreo",
        "Mini cheesecakes de maracuyá",
        "Triple: palta, tomate y huevo"
    }),
    new("pv-4","Pack Variado #4","variado",444,"50–65 personas",1116, new[]{
        "Mini pan con salchicha huachana",
        "Petipán de pollo (con/sin apio)",
        "Mini croissant mixto",
        "Mini pizzas",
        "Tequeños de jamón y queso",
        "Mini pan con asado",
        "Mini alfajores",
        "Piononitos de manjar",
        "Orejitas",
        "Bolitas crocantes de queso",
        "Triple: pollo, jamón y queso",
        "Mini croissant de pollo",
        "Mini Cheese & Bacon Burger",
        "Mini chocotorta argentina",
        "Trufas de maní",
        "Trufas de Oreo"
    }),
    new("pv-5","Pack Variado #5","variado",650,"70–85 personas",1527, new[]{
        "Tequeños de queso + crema de palta",
        "Mini croissant de huevo, jamón y queso",
        "Empanaditas de pollo",
        "Mini frankfurter",
        "Mini galletas de choco chips",
        "Conitos de manjar"
    }),
    new("pv-6","Pack Variado #6","variado",850,"90–100 personas",1945, new[]{
        "Mini croissant de huevo, jamón y queso",
        "Mini croissant de pollo",
        "Empanaditas de pollo",
        "Enrrolladitos de hot dog",
        "Shot de causitas de pollo (2 oz)",
        "Spring rolls de pollo, col, zanahoria y pimiento"
    }),
};

var packsSalados = new List<Pack> {
    new("ps-1","Pack Salado #1","salado",124,"15–20 personas",318, new[]{
        "Empanaditas de pollo o carne",
        "Enrrolladitos de hot dog",
        "Mini capresse",
        "Alitas BBQ",
        "Spring rolls de vegetales"
    }),
    new("ps-2","Pack Salado #2","salado",198,"25–30 personas",590, new[]{
        "Mini pan con salchicha huachana",
        "Mini pan con asado de res",
        "Mini croissant mixto",
        "Triple: palta, tomate y queso",
        "Mini pizzas"
    }),
    new("ps-3","Pack Salado #3","salado",325,"35–40 personas",850, new[]{
        "Empanaditas de pollo o carne",
        "Mini croissant de pollo",
        "Mini chicken crispy & cheese burger",
        "Mini pancitos con chicharrón",
        "Tequeños de jamón y queso",
        "Bolitas crocantes de queso",
        "Mini pan con pavo",
        "Petipán especial de pollo con pecanas",
        "Triple: jamón, pollo y huevo",
        "Mini butifarras"
    }),
    new("ps-4","Pack Salado #4","salado",448,"50–65 personas",1250, new[]{
        "Empanaditas de pollo/carne",
        "Petipán de pollo (con/sin apio)",
        "Mini pizzas",
        "Mini frankfurter",
        "Tequeños de jamón y queso",
        "Spring rolls de vegetales",
        "Mini francés (jamón, queso, lechuga, tomate)",
        "Enrrolladitos de hot dog",
        "Triple: pollo y espinaca con queso crema",
        "Mini croissant mixto"
    }),
    new("ps-5","Pack Salado #5","salado",646,"70–85 personas",1590, new[]{
        "Bolitas crocantes de queso",
        "Petipán de pollo con durazno",
        "Mini butifarras",
        "Alitas Buffalo picantes",
        "Mini cheese burger",
        "Tequeños de queso",
        "Mini choripanes",
        "Mini croissant de queso crema y pimiento rostizado",
        "Shot de causitas de pollo 2 oz",
        "Spring rolls de pollo y vegetales"
    }),
    new("ps-6","Pack Salado #6","salado",850,"90–100 personas",2042, new[]{
        "Mini cheese burger",
        "Tequeños de queso",
        "Mini choripanes",
        "Mini croissant de queso crema y pimiento",
        "Shot de causitas de pollo 2 oz",
        "Spring rolls de pollo y vegetales"
    }),
};

// ---------- ENDPOINTS ----------
app.MapGet("/api/hello", () =>
    Results.Json(new { message = "Hola 👋, backend .NET listo", serverTime = DateTimeOffset.UtcNow })
);

app.MapGet("/api/catalog", () => Results.Json(new { categories, items }));

app.MapGet("/api/packs", (string? tipo) =>
{
    if (string.Equals(tipo, "variado", StringComparison.OrdinalIgnoreCase)) return Results.Json(packsVariados);
    if (string.Equals(tipo, "salado",  StringComparison.OrdinalIgnoreCase)) return Results.Json(packsSalados);
    return Results.Json(packsVariados.Concat(packsSalados));
});


app.MapPost("/api/quote", (QuoteRequest req) =>
{
    var lines = new List<QuoteLine>();
    decimal total = 0;

    foreach (var it in req.Items)
    {
        var item = items.FirstOrDefault(x => x.Id == it.Id);
        if (item is null) continue;
        var tier = item.PriceTiers.FirstOrDefault(t => t.Qty == it.Qty);
        if (tier is null) continue;
        lines.Add(new QuoteLine($"{item.Name} × {tier.Qty}{(tier.UnitLabel is not null ? " " + tier.UnitLabel : "")}", tier.Price));
        total += tier.Price;
    }

    foreach (var pk in req.Packs)
    {
        var pack = packsVariados.Concat(packsSalados).FirstOrDefault(x => x.Id == pk.Id);
        if (pack is null) continue;
        var sub = pack.Price * pk.Count;
        lines.Add(new QuoteLine($"{pack.Name} × {pk.Count}", sub));
        total += sub;
    }

    return Results.Json(new QuoteResponse(lines, total));
});

app.MapGet("/api/terms", () =>
{
    var terms = new {
        anticipacionHoras = 48,
        deliveryIncluido = false,
        esperaConductorMin = 10,
        comprobantes = "Boleta y/o Factura electrónica",
        minimos = "Elegir mínimo 25 o 24 unidades según el bocadito",
        pagos = new [] { "Transferencias bancarias", "Yape/Plin", "Tarjetas " },
        politicas = new [] { "No hacemos devolución de dinero una vez agendado el pedido." },
        notas = new [] { "Consultar costos de delivery por zona.", "Consultar recojo en taller." },
        contacto = new { ciudad = "Santiago de Surco, Lima – Perú", telefono = "948 878 204", email = "vntas.flordecanela@gmail.com" }
    };
    return Results.Json(terms);
});

app.MapFallbackToFile("index.html");
app.Run();

// Cotizador: solo cantidades exactas según carta
record QuoteReqItem(string Id, int Qty);
record QuoteReqPack(string Id, int Count);
record QuoteRequest(List<QuoteReqItem> Items, List<QuoteReqPack> Packs);
record QuoteLine(string Label, decimal Subtotal);
record QuoteResponse(List<QuoteLine> Lines, decimal Total);

// ===================== DECLARACIONES DE TIPOS / HELPERS =====================
record PriceTier(int Qty, decimal Price, string? UnitLabel = null);
record Item(string Id, string Name, string CategoryId, string[]? Tags, PriceTier[] PriceTiers, string? Notes = null, string? Image = null);
record Category(string Id, string Name);
record Pack(string Id, string Name, string Type, int Pieces, string Persons, decimal Price, string[]? Items);

// Helpers de precios en clase estática (evita errores de top‑level)
static class Tiers
{
    public static PriceTier[] T25(decimal p25, decimal p50, decimal p100)
        => new[] { new PriceTier(25, p25), new PriceTier(50, p50), new PriceTier(100, p100) };

    public static PriceTier[] T24(decimal p24, decimal p48, decimal p100)
        => new[] { new PriceTier(24, p24), new PriceTier(48, p48), new PriceTier(100, p100) };

    public static PriceTier[] T24_96(decimal p24, decimal p48, decimal p96)
        => new[] { new PriceTier(24, p24), new PriceTier(48, p48), new PriceTier(96, p96) };

    public static PriceTier[] T12_24_48_96(decimal p12, decimal p24, decimal p48, decimal p96)
        => new[] { new PriceTier(12, p12), new PriceTier(24, p24), new PriceTier(48, p48), new PriceTier(96, p96) };

    public static PriceTier[] Cake(decimal price)
        => new[] { new PriceTier(1, price, "cake (21×11×7 cm)") };
}

