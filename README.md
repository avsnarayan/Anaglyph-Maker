# Anaglyph-Maker
 Prosta aplikacja napisana w środowisku .NET z użyciem asemblera do tworzenia anaglifów z dwóch obrazów.

# Anaglify

Wrażenie widzenia przestrzennego obrazu powstaje w mózgu na skutek analizowania różnic w obrazie z lewego i prawego oka. Do tego służą specjalne okulary i zastosowane w nich filtry: czerwony (oko lewe) i niebieski (oko prawe). Filtr czerwony przepuszcza tylko barwę czerwoną, a niebieski barwę niebieską.  Posiadając dwa obrazy zrobione aparatem z dwoma obiektywami, które przesunięte są w poziomie pod małym kątem, można uzyskać anaglif kolorując odpowiednio obraz lewy na odcień czerwony, prawy na odcień zielonkawo-niebieski, a następnie je scalić. Istnieje  kilka wersji anaglifów. W programie dostępne są następujące wersje: true anaglyph, gray anaglyph, color anaglyph, half-color anaglyph, optimized anaglyph.

Algorytm zastosowany w programie prezentuje się następująco: 
 
A_red = F_R0 * L_red + F_R1 * L_green + F_R2 * L_blue 

A_green = F_G0 * R_red + F_G1 * R_green + F_G2 * R_blue 

A_blue = F_B0 * R_red + F_B1 * R_green + F_B2 * R_blue 

gdzie: 
 
L – lewy obraz (Left)

R – prawy obraz (Right) 

A – anaglif (Anaglyph) 

F – filtr (Filter) 

# Opis programu

Program został napisany przy pomocy zintegrowanego środowiska programistycznego Visual Studio 2019 Community. W celu stworzenia graficznego środowiska użytkownika posłużono się Windows Forms App, które oferuje .NET Framework. Obsługa GUI jest w całości napisana w języku wysokiego poziomu C#. Natomiast algorytm tworzenia anaglifu został napisany w języku C++ oraz w asemblerze. Kody programu odpowiedzialne za tworzenie anaglifu są dołączane do aplikacji w postaci dwóch bibliotek DLL.

W celu wygenerowania anaglifu, zadaniem użytkownika jest umieszczenie dwóch odpowiednich obrazów (przykładowo zrobionych specjalnym aparatem z dwoma obiektywami lub szyną), które mają być scalone w obraz-anaglif. Formaty obrazów akceptowanych przez aplikację to: .PNG, .JPG, .BMP. Wybrane przez użytkownika zdjęcia (lewe i prawe) muszą być takiej samej wysokości oraz szerokości. W przypadku różnych rozmiarów, program wyświetli komunikat o niezgodnych obrazach. 

Aplikacja umożliwia wybór pięciu rodzajów anaglifu: 
 
True (prawdziwy anaglif) – powstały obraz jest głównie w odcieniu fioletowym i jego zadaniem jest przedstawienie głębi. 
 
Gray (czarno-biały anaglif) – powstały obraz jest w odcieniach szarości i posiada wyraźną głębię 
 
Color (kolorowy anaglif) – powstały obraz jest kolorowy (doświadczenie głębi jest zaburzone) 
 
Half color (anaglif w pół kolorze) – powstały obraz traci częściowo informacje o kolorach, ale za to odczucie głębi posiadanej przez obraz nie jest zaburzone 
 
Optimized (zoptymalizowany anaglif) – powstały obraz jest pośrednim przejściem pomiędzy Color a Half color 
 
Dodatkowo zaimplementowano radio buttony do wyboru rodzaju DLL. Aplikacja domyślnie ustala optymalną liczbę wątków dla maszyny użytkownika, ale użytkownik może tą liczbę dowolnie zmieniać od 1 do 64 suwakiem lub wpisując liczbę z tego przedziału w okienku obok. 
 
Po kliknięciu przycisku „Generate anaglyph” aplikacja połączy dwa zdjęcia tworząc anaglif. Otrzymany wynik jest zapisywany w postaci bitmapy do folderu, w którym znajduje się plik wykonywalny (.exe). 

# Utworzone biblioteki

Biblioteka napisana w języku C++ wykorzystuje instrukcje wektorowe SIMD. Zawiera jedną funkcję AnaglyphAlgorithm(float* leftImage, int* coords, float* filters, float* rightImage) typu void przyjmującą 4 parametry: 
 
leftImage – wskaźnik typu float (tablica) wskazujący na pierwszą wartość piksela lewego obrazu. 
 
coords – wskaźnik typu int (tablica 2-elementowa). Parametr ten umożliwia dostęp do wartości indeksu tablicy OD którego algorytm ma pracować oraz wartości indeksu DO którego algorytm ma pracować. Indeksy te są wykorzystywane w celu podziału pracy między wątki. 
 
filters – wskaźnik typu float (tablica). Jest to filtr zawierający wartości od 0 do 1 (liczby zmiennoprzecinkowe). Przez te wartości mnożone są wartości pikseli obrazu lewego i prawego. 
 
rigthImage - wskaźnik typu float (tablica) wskazujący na pierwszą wartość piksela prawego obrazu. 
 
Do przechowywania wartości i ich modyfikacji użyto dostępnych zmiennych typu __m128 (4x float). 

Algorytm tworzenia anaglifów napisany w asemblerze korzysta z instrukcji wektorowych SIMD oraz rejestrów xmm0-xmm7. Był implementowany w ten sposób, by być jak najbardziej spójnym z algorytmem pisanym w C++.  Parametry przekazywane są przez 4 rejestry: 
 
RCX – wartości pikseli lewego obrazu RDX – wartości indeksów początku i końca od których algorytm ma wykonywać operacje na tablicy pikseli obrazów R8 – wartości filtru anaglifu R9 – wartości pikseli prawego obrazu 
 
W obu przypadkach wywoływania bibliotek (C++ DLL oraz Asm DLL), takie same parametry w programie głównym przekazywane są w ten sam sposób - poprzez wskaźniki.

# Wygląd aplikacji

![GitHub Logo](/images/anaglyphMaker1.jpg)
