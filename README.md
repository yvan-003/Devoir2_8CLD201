# Projet Azure DevOps : Traitement Automatisé des Images

## Description
Ce projet vise à automatiser le traitement d'images en utilisant les services d'Azure, notamment :
- **Azure Storage Account** : pour stocker les fichiers d'images.
- **Azure Function Apps** : pour automatiser les tâches liées au traitement des images.
- **Azure Bus Queue** : pour gérer la communication entre les différentes étapes.
- **Pipelines CI/CD** : pour automatiser la validation, la compilation, et le déploiement.

## Fonctionnalités
- Détection automatique de nouveaux fichiers déposés dans le Storage Account.
- Envoi du nom des fichiers à une Azure Bus Queue.
- Traitement des fichiers (redimensionnement ou ajout de watermark).
- Déplacement des fichiers traités vers un conteneur dédié et suppression des fichiers originaux.
- Déploiement automatisé avec Azure DevOps Pipelines.

## Prérequis
Avant de commencer, assurez-vous d'avoir :
1. Un compte Azure actif.
2. Un compte Azure DevOps.
3. Les outils suivants installés localement :
   - [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
   - [Python 3.x](https://www.python.org/downloads/) avec les bibliothèques nécessaires.
   - [Git](https://git-scm.com/).

## Contributeurs

- SEKA paul yvan : Gestion du projet, organisation du Board.
- [Nom du membre 2] : Développement de la première Function App.
- [Nom du membre 3] : Développement de la deuxième Function App.
- [Nom du membre 4] : Configuration des pipelines CI/CD.


### Points forts de ce README :
- **Clarté** : La structure est intuitive et facile à suivre.
- **Documentation technique** : Chaque étape clé du projet est expliquée.
- **Collaboratif** : Mention des contributeurs.
- **Professionnel** : Utilisation d'une licence et de bonnes pratiques.
