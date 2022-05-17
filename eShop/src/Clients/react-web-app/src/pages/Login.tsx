import { useForm, SubmitHandler } from "react-hook-form";

type Inputs = {
    email: string,
    password: string,
    rememberMe: boolean
};

const Login = () => {
    const { register, handleSubmit } = useForm<Inputs>();
    const onSubmit: SubmitHandler<Inputs> = data => console.log(data);

    return (
        <div className="h-[100vh] bg-sky-600 flex">
            <div className="block p-6 rounded-lg shadow-lg bg-white max-w-sm m-auto sm:w-96">
                <div className="block mt-2 mb-6 text-center">
                    <span className="w-[70px] h-[70px] flex p-4 border-2 border-gray-200 rounded-full m-auto">
                        <i className="fa-solid fa-user text-3xl m-auto"></i>
                    </span>
                </div>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <div className="form-group mb-6">
                        <label htmlFor="exampleInputEmail2" className="form-label inline-block mb-2 text-gray-700">Email address</label>
                        <input {...register("email", { required: true })} type="email" id="exampleInputEmail2" aria-describedby="emailHelp" placeholder="Enter email" autoComplete="on" className="form-control   
                            block 
                            w-full 
                            px-3 
                            py-1.5 
                            text-base 
                            font-normaltext-gray-700bg-white 
                            bg-clip-padding
                            border border-solid 
                            border-gray-300
                            rounded transition 
                            ease-in-out m-0  
                            focus:outline-none
                            focus:text-gray-700 
                            focus:bg-white
                            focus:border-blue-600"
                        />
                    </div>
                    <div className="form-group mb-6">
                        <label htmlFor="exampleInputPassword2" className="form-label inline-block mb-2 text-gray-700">Password</label>
                        <input {...register("password")} type="password" id="exampleInputPassword2" placeholder="Password" autoComplete="on" className="form-control block
                            w-full
                            px-3
                            py-1.5
                            text-base
                            font-normal
                            text-gray-700
                            bg-white bg-clip-padding
                            border border-solid 
                            border-gray-300
                            rounded
                            transition
                            ease-in-out
                            m-0
                            focus:text-gray-700
                            focus:bg-white
                            focus:border-blue-600
                            focus:outline-none"
                        />
                    </div>
                    <div className="flex justify-between items-center mb-6">
                        <div className="flex">
                            <input {...register("rememberMe")} type="checkbox" id="exampleCheck2" className="h-4 w-4 mt-1 mr-2 
                                border rounded-sm border-gray-300 bg-white checked:bg-blue-600 checked:border-blue-600
                                transition duration-200 cursor-pointer"
                            />
                            <label className="form-check-label inline-block text-gray-800" htmlFor="exampleCheck2">Remember me</label>
                        </div>
                        <a href="#!" className="text-blue-600 hover:text-blue-700 focus:text-blue-700 transition duration-200 ease-in-out">
                            Forgot password?
                        </a>
                    </div>
                    <button type="submit" className="w-full
                        px-6
                        py-2.5
                        bg-blue-600
                        text-white
                        font-medium
                        text-xs
                        leading-tight
                        uppercase
                        rounded
                        shadow-md
                        hover:bg-blue-700 hover:shadow-lg
                        focus:bg-blue-700 focus:shadow-lg focus:outline-none focus:ring-0
                        active:bg-blue-800 active:shadow-lg
                        transition
                        duration-150
                        ease-in-out">Sign in</button>
                    <div className="text-gray-800 mt-6 flex justify-between">
                        <span>Not a member?</span>
                        <a href="#!" className="text-blue-600 hover:text-blue-700 focus:text-blue-700 transition duration-200 ease-in-out">Register</a>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default Login;