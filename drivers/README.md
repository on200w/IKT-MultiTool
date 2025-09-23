# Drivere

Denne mappen inneholder drivere som kan lastes ned gjennom IKT MultiTool.

## Struktur

Mappene opprettes automatisk når du legger til nye PC-modeller gjennom programmet.

Eksempel:
- `Lenovo_Thinkpad_E14_Gen6/` - Drivere for Lenovo Thinkpad E14 Gen6

## Instruksjoner

1. Åpne IKT MultiTool og gå til "Drivere"
2. Hvis det ikke finnes PC-modeller, klikk på "Legg til ny PC-modell"
3. Skriv inn navnet på PC-modellen (f.eks: "Lenovo Thinkpad E15 Gen5")
4. Velg PC-modellen fra listen
5. Klikk på "Åpne drivermappe for denne PC-en"
6. Plasser driverfiler (.exe, .msi, .zip) i mappen
7. Gå tilbake til hovedmenyen og inn på PC-modellen igjen for å se de nye driverne
8. Klikk på en driver for å installere den

## Støttede filtyper

- .exe (Kjørbare installasjonsfiler)
- .msi (Windows Installer pakker)
- .zip (Komprimerte arkiver)

## Når programmet publiseres

Inkluder hele `drivers`-mappen med innholdet når du publiserer programmet.

## Administrasjon

- Nye PC-modeller kan legges til direkte gjennom programmet
- Mapper får automatisk navn basert på PC-modellnavnet
- Spesialtegn og mellomrom konverteres til understrek (_)
