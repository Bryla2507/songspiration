@echo off
echo Starting SongSpiration applications...

start cmd /k "cd backend/SongSpiration.API && dotnet run"
timeout /t 5 /nobreak

start cmd /k "cd frontend && npm run dev"

echo Applications should be starting...
echo Backend: http://localhost:5036
echo Frontend: http://localhost:5173 (or check console for actual port)