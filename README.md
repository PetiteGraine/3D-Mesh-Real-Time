# 3D-Mesh-Real-Time
## Partie 1 : Génération d’un Terrain Procédural
### 1.2 Générer une grille de vertices
#### Combien de vertices pour une grille 50x50 ?
Pour une grille de 50x50, le nombre total de vertices est 50 * 50 = 2500.

#### Comment placer les vertices pour qu’ils soient espacés régulièrement ?
Pour espacer les vertices régulièrement, il faut multiplier les coordonnées (x, z) par un facteur d'espacement

### 1.3 Générer l’altitude avec Perlin Noise
#### Que se passe-t-il si scale est trop grand ou trop petit ?
- **Si trop grand :** Les valeurs de Perlin Noise changent très lentement, ce qui rend le terrain très lisse et plat, avec des variations de hauteur peu fréquentes.
- **Si trop petit :** Les valeurs de Perlin Noise changent très rapidement, ce qui crée un terrain très chaotique avec des variations de hauteur très rapprochées, donnant un aspect bruité.

#### Comment rendre le terrain plus montagneux ?
Pour rendre le terrain plus montagneux, il faut augmenter l'amplitude des variations de hauteur.

### 1.4 Connecter les vertices en triangles
#### Combien de triangles pour une grille 50x50 ?
**Nombre de cellules :** (50 - 1) x (50 - 1) = 49 x 49 = 2401.
**Nombre de triangles :** 2401 x 2 = 4802.

#### Comment parcourir la grille pour relier correctement les indices ?
Pour relier correctement les indices, il faut parcourir chaque cellule de la grille et définir les deux triangles qui la composent. Voici comment cela fonctionne :

**Indices des sommets d'une cellule :**
- bottomLeft : Sommet inférieur gauche.
- topLeft : Sommet supérieur gauche.
- topRight : Sommet supérieur droit.
- bottomRight : Sommet inférieur droit.

**Triangles dans une cellule :**
Premier triangle : bottomLeft, topLeft, topRight.
Deuxième triangle : bottomLeft, topRight, bottomRight.

**Parcours de la grille :**
Parcourez les cellules avec deux boucles imbriquées (pour z et x).
Calculez les indices des sommets pour chaque cellule.
Ajoutez les indices des deux triangles dans le tableau triangles.

## Partie 2 : Ajout de Collision MeshCollider Dynamique
### 2.3 Convexité du Collider
#### Quelle est la différence entre un collider convex et non convex ?
**Collider Convex :**
Un collider convex est une forme simplifiée où tous les points à l'intérieur du collider peuvent être reliés sans sortir de la forme.
Il est optimisé pour les calculs physiques, mais ne peut pas représenter des formes complexes ou concaves.
Utilisé principalement pour des objets simples ou mobiles.

**Collider Non Convex :**
Un collider non convex peut représenter des formes complexes, y compris des trous ou des parties concaves.
Il est plus coûteux en termes de calculs physiques.
Utilisé pour des objets statiques ou des formes détaillées.

#### Pourquoi ne pas rendre un terrain entier convex ?
- **Perte de précision :** Un terrain est généralement une forme complexe avec des creux et des bosses. Si on le rend convex, ces détails sont perdus, et le collider devient une enveloppe simplifiée qui ne correspond pas à la topographie réelle.
- **Problèmes de gameplay :** Les objets ou personnages pourraient "flotter" au-dessus des creux ou traverser des parties du terrain, car le collider ne correspondrait pas à la surface réelle.
- **Performance inutile :** Les terrains sont souvent statiques et utilisent un MeshCollider non convex, qui est plus précis pour représenter des formes complexes.
En résumé, un terrain entier ne devrait pas être convex, car cela compromettrait la précision et l'expérience utilisateur. Un MeshCollider non convex est plus adapté pour les terrains.

### 3.1 Capturer un clic souris
#### Comment déterminer les vertices proches du point cliqué ?
Après avoir obtenu le point d'impact du clic, on peut parcourir tous les vertices du terrain et calculer leur distance par rapport au point d'impact. Si la distance est inférieure à un rayon donné, ces vertices sont considérés comme proches.

#### Quelle distance autour du hit appliquer la déformation ?
On prends tous les vertices considérés comme proche, sur ces vertices on rajoute sur l'axe y une valeur et on refait le mesh à partir des nouvelles valeurs des vertices.

### 4.1 Mise en place d'un LOD simple
#### Pourquoi utiliser des Jobs dans ce contexte ?
- **Parallélisme :** Le Job System permet d'exécuter des boucles (comme for) sur plusieurs threads CPU en parallèle. Au lieu de traiter chaque vertex un par un sur le thread principal, on peut répartir la charge sur plusieurs cœurs.
- **Performances accrues :** Le Burst Compiler compile le code C# en instructions machine ultra-optimisées. Cela peut considérablement accélérer les calculs mathématiques répétitifs (comme ici, la distance entre points et l'ajustement des hauteurs).