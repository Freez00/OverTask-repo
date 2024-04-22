<script lang='ts'>
    import { onMount } from 'svelte';
    import Typed from 'typed.js';
    import Navbar from './navbar.svelte';
    import {goto} from '$app/navigation';

    
    let typed;
    
    onMount(() => {
      typed = new Typed("#typed", {
        strings: ["Добре дошли в Overtask!", "Разпределете ангажиментите си в календара!", "Създавайте списъци със задачи!", "Създавайте групи!", "Приятно планиране!"],
        typeSpeed: 110,
        startDelay: 500,
        loop: true,
        loopCount: Infinity,
        backDelay: 1000
      });
    });
    </script>

<div class="all">

  <Navbar></Navbar>
  <div class="container-fluid position-relative overflow-hidden mb-4 mb-sm-0 linear-gradient-body mainDiv">
    
    <div class="container mt-xl-5 mt-lg-5">
      <div class="row pt-xl-5 pt-lg-5 d-sm-flex flex-column-reverse flex-lg-row text-center text-md-start">
        <div class="col-xl-6 col-lg-7 pt-xl-5 mt-xl-5 mt-lg-5">
          <h3 class="h3 fw-semibold opacity-75">
            <!--       TYPED JS       -->
            <span style="font-weight:bold;" id="typed"></span>
          </h3>
          
          <h1 class="h1 fw-bold">Организирайте <span class="d-block d-sm-inline px-2 text-white rounded" style="background-color:#422a57">живота</span> си днес!</h1>
          <p class="fs-6 fw-medium w-75 width-100-important opacity-75">Overtask ви позволява да създавате списъци със задачи и да
            разпределяте ангажиментите си в календар. Направете група за организиране на групови събития и други.</p>
            <a href="#" class="btn text-white fs-5" style="background-color:#EA718C;" on:click={async () => await goto("/todo")}>Започнете сега</a>
          </div>
          <div class="col-xl-6 col-lg-5 mt-lg-5 position-relative">
            <img src="calendar2.webp" class="moving-image">
          </div>
        </div>
      </div>
    </div>    
</div>
<style>
*{
    font-family: 'Cabin', sans-serif; 
}
.all {
  display: flex;
  flex-direction: column;
  height: 100vh;
  overflow: auto;
  min-height: 700px;
}

.mainDiv {
  flex-grow: 1;
  margin:0px !important;
}
.fw-medium {
  font-weight: 500;
}
.fw-semibold {
  font-weight: 600;
}
.fw-bold {
  font-weight: 700;
}
.linear-gradient-body {
  background: linear-gradient(287.83deg, #916cbe, #d9cce8);
}
/* .linear-gradient-body {
  background: linear-gradient(
    287.83deg,
    #8d99ae 32.33%,
    rgba(237, 242, 244, 0) 91.61%
  );
} */

@media only screen and (max-width: 600px) {

  .moving-image{
    margin-top:30px;
    width:300px;
  }
}

@media only screen and (max-width: 768px) {
  .width-100-important {
    width: 100% !important;
  }
  .moving-image{
    margin-top:40px;
    width:400px;
  }
}

@media only screen and (min-width: 992px) {
  .moving-image{
    margin-top:60px;
    width:600px;
  }
}

@media only screen and (min-width: 1200px) {
  .moving-image{
    width:auto;
  }
}

@keyframes up-and-down {
    0% { transform: translateY(0); }
    50% { transform: translateY(-20px); }
    100% { transform: translateY(0); }
  }
  .moving-image {
    margin-top: 30px;
    animation: up-and-down 3s infinite ease-in-out;
  }

</style>