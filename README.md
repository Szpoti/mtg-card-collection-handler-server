# MTG Card Collection Handler (back-end)

<img src=".github/navbar.jpg">

## Introduction

MTG Card Collection Handler is a website where You can browse through the wise collection of cards released by Wizards, makers of the famous card game [Magic: the Gathering](https://magic.wizards.com/).

> This repository holds only the server-side code written with C# as an ASP.NET web API.
>
> If You want to interact with it with a nice UI, [setup the front-end](https://github.com/Szpoti/mtg-card-collection-handler-ui#setup) too.

## Technologies

- ASP.NET Core 3.1
- Entity Framework Core 3.1 with
  - PostgreSQL provider

## Setup

### PostreSQL

Create a user with a name `postgres` and password `mksm20`.

Create the necessary database and tables

```sh
$ dotnet ef database update
```

### Web API

`cd` into the `MagicApi` subfolder

Start the webserver

```sh
$ dotnet run
```

## Status

- [x] **Quick search**: _Search for cards by its name and colors_
- [x] **Advanced search**: _Search for cards by its name, colors, type, subtype, minimum and maximum price (\$,€) and artist name_
- [x] **Registration**: _Password hashed with BCrypt_
- [x] **Login**: _Auth with JWT_
- [ ] **Deck creation**: _Build decks from cards_
- [ ] **Card market**: _Place to trade cards between users with real money_
