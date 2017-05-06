package com.example.athis.myapplication;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.StrictMode;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.TextView;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;

public class MainActivity extends AppCompatActivity {
    View view;
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        /*
        * 主Activity加载自动运行代码
        * */
        EditText editText1 = (EditText)findViewById(R.id.username);
        EditText editText2 = (EditText)findViewById(R.id.pwd);
        CheckBox checkBox = (CheckBox)findViewById(R.id.auto);
        editText1.setText(getUserName());
        editText2.setText(getPwd());
        checkBox.setChecked(getAuto());
        auto(view);
    }

    public void login(View view)
    {
        /*
        * 登陆功能
        * */
        if(notEmpty()) {
            String username = "", pwd = "";
            EditText editText1 = (EditText) findViewById(R.id.username);
            EditText editText2 = (EditText) findViewById(R.id.pwd);
            username = editText1.getText().toString();
            pwd = editText2.getText().toString();
            String url = "http://222.179.99.144:8080/eportal/webGateModeV2.do?method=login&mac=0026c7609610&t=wireless-v2-plain";
            String nasip = isInternet();
            url = url + "&username=" + username + "&pwd=" + pwd + "&" + nasip;
            sendGet(url);

            //提示登陆结果
            TextView textView = (TextView) findViewById(R.id.message);
            if (isInternet() == "") {
                textView.setText("恭喜，登陆成功");
            } else {
                textView.setText("很抱歉，登陆失败");
            }
            saveData();//存储账号密码
        }
    }

    public void logout(View view)
    {
        /*
        * 注销功能
        * */
        String url = "http://222.179.99.144:8080/eportal/userV2.do?method=offline";
        sendGet(url);
        TextView textView = (TextView) findViewById(R.id.message);
        if(isInternet()!=""){
            textView.setText("恭喜，注销成功");
        }else {
            textView.setText("很抱歉，注销失败");
        }
    }

    public void auto(View view){
        /*
        * 自动登陆功能
        * */
        CheckBox checkBox = (CheckBox)findViewById(R.id.auto);
        if(checkBox.isChecked()){
            login(view);
        }
        saveData();
    }

    public boolean notEmpty(){
        /*
        *判断用户名与密码是否为空
        * */
        EditText editText1 = (EditText)findViewById(R.id.username);
        EditText editText2 = (EditText)findViewById(R.id.pwd);
        if(editText1.getText().toString().equals("")){
            new  AlertDialog.Builder(this).setTitle("警告").setMessage("请输入用户名").setPositiveButton("确定" ,  null ).show();
            System.out.print("false1");
            return  false;
        }else if(editText2.getText().toString().equals("")){
            new  AlertDialog.Builder(this).setTitle("警告").setMessage("请输入密码").setPositiveButton("确定" ,  null ).show();
            System.out.print("false2");
            return  false;
        }
        return true;
    }

    public static String sendGet(String url) {
        /*
        * 封装Get方法
        * */
        String result = "";
        BufferedReader in = null;
        try {
            if (android.os.Build.VERSION.SDK_INT > 9) {
                /*
                * 强制访问网络,必加！！
                * */
                StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
                StrictMode.setThreadPolicy(policy);
            }
            URL realUrl = new URL(url);
            // 打开和URL之间的连接
            URLConnection conn = realUrl.openConnection();
            // 建立实际的连接
            conn.connect();
            // 获取所有响应头字段
            // 定义BufferedReader输入流来读取URL的响应
            in = new BufferedReader(
                    new InputStreamReader(conn.getInputStream()));
            String line;
            while ((line = in.readLine()) != null) {
                result = result + line;
            }
        } catch (Exception e) {
            return "发送GET请求出现异常"+ e;
        }
        // 使用finally块来关闭输入流
        finally {
            try {
                if (in != null) {
                    in.close();
                }
            } catch (IOException ex) {
                ex.printStackTrace();
            }
        }
        return result;
    }
    public String isInternet(){
            /*
            访问百度，如果成功返回空字符串；如果重定向跳转登陆界面，返回nasip
             */
        String url = "http://www.baidu.com";
        String retString = sendGet(url);
        String[] spilt = retString.split("&");
        for (int x=0;x<spilt.length;x++)
        {
            System.out.print(spilt[x]);
            if (spilt[x].indexOf("nasip") != -1)
            {
                return spilt[x];
            }
        }
        return "";
    }
    public void saveData(){
        TextView textView1 = (TextView)findViewById(R.id.username);
        TextView textView2 = (TextView)findViewById(R.id.pwd);
        CheckBox checkBox = (CheckBox)findViewById(R.id.auto);
        String username = textView1.getText().toString();
        String pwd = textView2.getText().toString();
        boolean auto = checkBox.isChecked();
        SharedPreferences sp;
        sp = this.getSharedPreferences("data", 0);
        SharedPreferences.Editor editor = sp.edit();
        editor.putString("username", username);
        editor.putString("pwd", pwd);
        editor.putBoolean("auto", auto);
        editor.commit();
    }
    public String getUserName(){
        SharedPreferences sp;
        sp = this.getSharedPreferences("data", 0);
        return sp.getString("username","");
    }
    public String getPwd(){
        SharedPreferences sp;
        sp = this.getSharedPreferences("data", 0);
        return sp.getString("pwd","");
    }
    public boolean getAuto(){
        SharedPreferences sp;
        sp = this.getSharedPreferences("data", 0);
        return sp.getBoolean("auto",false);
    }
}
