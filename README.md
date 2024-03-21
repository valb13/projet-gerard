# Recup-projet-gerard

Documentation
---------------

Image docker à installer pour la base redis compatible avec le package utilisé dans le projet :

$ docker run -p 10001:6379 -p 13333:8001 redis/redis-stack:latest

Assurez-vous que le conteneur est en cours d'exécution pour pouvoir utiliser l'application.

Le projet repose sur le framework Blazor ainsi que sur le langage C#. Il adopte une architecture conforme au modèle MVC.
La partie backend du projet est située dans le répertoire "/Server". À cet emplacement, vous trouverez plusieurs éléments essentiels :

    Le contrôleur "ProductsController.cs", responsable de la gestion des requêtes HTTP relatives aux produits. Il orchestre les actions nécessaires pour récupérer, créer, mettre à jour ou supprimer des données liées aux produits dans la base de données.

    L'interface "IProducts.cs", qui définit les fonctions et méthodes nécessaires pour interagir avec la base de données en ce qui concerne les produits. Cette interface sert de contrat, garantissant une implémentation cohérente et uniforme des opérations liées aux produits.

    Le service "ProductsDataAccess.cs", conçu pour établir et gérer la connexion avec la base de données, ainsi que pour exécuter les requêtes et les transactions nécessaires pour accéder et manipuler les données des produits. Ce service encapsule la logique d'accès aux données, favorisant ainsi la modularité et la réutilisabilité du code.

Tous ces fichiers sont minutieusement commentés pour fournir une documentation claire et concise sur le fonctionnement de chaque fonctionnalité, facilitant ainsi la compréhension et la maintenance du code pour les développeurs.
