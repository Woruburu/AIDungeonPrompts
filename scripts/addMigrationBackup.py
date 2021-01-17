import sys, subprocess

if len(sys.argv) < 2 or not sys.argv[1]:
	raise Exception("Please supply a migration name") 

subprocess.call(["dotnet", "ef", "migrations", "add", sys.argv[1], "--project", ".\\AIDungeonPrompts.Backup.Persistence\\", "--startup-project", ".\\AIDungeonPromptsWeb\\", "--context", "BackupDbContext"])