@echo off
cls
ECHO Atualizando Service-Bills-Paid
echo[

ECHO Parando servico
sc.exe stop Service-Bills-Paid-1.0

D:

echo[
ECHO ---------- Indo para o diretorio

cd D:\projects\bills-service\Bills.Service.Host

echo[
ECHO ---------- Publicando/Buildando
dotnet publish -o D:\projects\bills-service\Bills.Service.Host\publish

echo[
ECHO ---------- Tentando criar o servico
sc.exe create Service-Bills-Paid-1.0 binpath=D:\projects\bills-service\Bills.Service.Host\publish\Bills.Service.Host.exe
timeout 10
echo[
ECHO ---------- Iniciando o Servico
sc.exe start Service-Bills-Paid-1.0