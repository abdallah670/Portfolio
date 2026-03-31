import sqlite3
import os

conn = sqlite3.connect('portfolio.db')
cursor = conn.cursor()
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name != '__EFMigrationsHistory'")
tables = cursor.fetchall()

type_map = {
    'INTEGER': 'int',
    'TEXT': 'string',
    'REAL': 'double',
    'NUMERIC': 'decimal',
    'BLOB': 'byte[]'
}

os.makedirs('Domain/Entities', exist_ok=True)

for (table_name,) in tables:
    class_name = table_name
    if class_name.endswith('ies'):
        class_name = class_name[:-3] + 'y'
    elif class_name.endswith('s') and class_name != 'HeroStats':
        class_name = class_name[:-1]
    
    if class_name == 'Heroe': class_name = 'Hero'

    cursor.execute(f"PRAGMA table_info('{table_name}')")
    columns = cursor.fetchall()
    
    props = []
    for col in columns:
        cid, name, ctype, notnull, dflt_value, pk = col
        csharp_type = "string"
        for k, v in type_map.items():
            if k in ctype.upper():
                csharp_type = v
                break
        
        # specific fixes
        if name.startswith('Is'): csharp_type = 'bool'
        if name in ['CreatedAt', 'UpdatedAt', 'ReadAt']: csharp_type = 'DateTime'
        
        nullable = "?" if (not notnull and csharp_type != 'string') or (not notnull and csharp_type == 'string') else ""
        if csharp_type == 'string' and not notnull: nullable = "?"
        if csharp_type == 'DateTime' and not notnull: nullable = "?"
        
        init = " = string.Empty;" if csharp_type == "string" and notnull else ""
        if name == 'CreatedAt' and notnull: init = " = DateTime.UtcNow;"
        
        props.append(f"    public {csharp_type}{nullable} {name} {{ get; set; }}{init}")
    
    content = f"using System;\n\nnamespace PortfolioApi.Domain.Entities;\n\npublic class {class_name}\n{{\n" + "\n".join(props) + "\n}\n"
    
    with open(f"Domain/Entities/{class_name}.cs", "w", encoding="utf-8") as f:
        f.write(content)

print('Entities recreated successfully!')
