node{
      stage ('Checkout'){
    checkout scm
    }

      stage ('Build'){
    bat 'nuget restore validation.sln'
    bat 'msbuild validation.sln /p:Configuration=Release'
    bat "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\binmsbuild validation.sln /p:Configuration=Release"
    }

      stage ('Archive'){
    archive '$WORKSPACE/build/Release/**'
    }
}
