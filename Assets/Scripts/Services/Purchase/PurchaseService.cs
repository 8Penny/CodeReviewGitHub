using System;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using Services.Analytics;
using Services.ShopService;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Zenject;

public class PurchaseService : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private static UnityEngine.Purchasing.Product test_product = null;

    IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    public static string NO_ADS = "no_ads";
    

    const string k_Environment = "production";
    //public static string MYSUB = "mysub";

    private Boolean return_complete = true;
    private event Action<int> ProductDetailsFailedAction;
    
    public Action Initialized;

    [Inject]
    public ShopService ShopService;

    private void Awake()
    {
        Initialize(OnSuccess, OnError);
        
        ShopService.Register(this);
    }
    
    
    void Initialize(Action onSuccess, Action<string> onError)
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(k_Environment);

            UnityServices.InitializeAsync(options).ContinueWith(task => onSuccess());
        }
        catch (Exception exception)
        {
            onError(exception.Message);
        }
    }

    void OnSuccess()
    {
        var text = "Congratulations!\nUnity Gaming Services has been successfully initialized.";
        Debug.Log(text);
    }

    void OnError(string message)
    {
        var text = $"Unity Gaming Services failed to initialize with error: {message}.";
        Debug.LogError(text);
    }

    async void Start()
    {
        InitializePurchasing();
    }
    
    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(OnDeferredPurchase);
        builder.Configure<IGooglePlayConfiguration>().SetQueryProductDetailsFailedListener(ProductDetailsFailedAction);


        foreach (var key in ShopService.ShopProducts.Keys)
        {
            builder.AddProduct(key, ProductType.Consumable);
        }
        foreach (var key in ShopService.ShopProductsList.Keys)
        {
            builder.AddProduct(key, ProductType.Consumable);
        }
        
        builder.AddProduct(NO_ADS, ProductType.NonConsumable);
        //builder.AddProduct(MYSUB, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }
    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    void OnDeferredPurchase(UnityEngine.Purchasing.Product product)
    {
        MyDebug($"Purchase of {product.definition.id} is deferred");
    }

    public void CompletePurchase()
    {
        if (test_product == null)
            MyDebug("Cannot complete purchase, product not initialized.");
        else
        {
            m_StoreController.ConfirmPendingPurchase(test_product);
            MyDebug("Completed purchase with " + test_product.transactionID.ToString());
        }

    }

    public void ToggleComplete()
    {
        return_complete = !return_complete;
        MyDebug("Complete = " + return_complete.ToString());

    }

    public void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            UnityEngine.Purchasing.Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                MyDebug(string.Format("Purchasing product:" + product.definition.id.ToString()));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                MyDebug("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            MyDebug("BuyProductID FAIL. Not initialized.");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        MyDebug("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        Initialized?.Invoke();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        MyDebug("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        MyDebug("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        test_product = args.purchasedProduct;

        //var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        //var result = validator.Validate(args.purchasedProduct.receipt);
        //MyDebug("Validate = " + result.ToString());

        if (m_GooglePlayStoreExtensions.IsPurchasedProductDeferred(test_product))
        {
            //The purchase is Deferred.
            //Therefore, we do not unlock the content or complete the transaction.
            //ProcessPurchase will be called again once the purchase is Purchased.
            return PurchaseProcessingResult.Pending;
        }
        if (return_complete)
        {
            
            if (test_product.definition.id == NO_ADS)
            {
                ShopService.PurchaseADSComplete();
            }
            else
            {
                ShopService.PurchaseSuccess(test_product.definition.id);
            }
            
            MyDebug(string.Format("ProcessPurchase: Complete. Product:" + args.purchasedProduct.definition.id + " - " + test_product.transactionID.ToString()));
            return PurchaseProcessingResult.Complete;
        }
        else
        {
            MyDebug(string.Format("ProcessPurchase: Pending. Product:" + args.purchasedProduct.definition.id + " - " + test_product.transactionID.ToString()));
            return PurchaseProcessingResult.Pending;
        }

    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
        ShopService.PurchaseFailed();
        MyDebug(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void ListPurchases()
    {
        foreach (UnityEngine.Purchasing.Product item in m_StoreController.products.all)
        {
            if (item.hasReceipt)
            {
                MyDebug("In list for  " + item.receipt.ToString());
            }
            else
                MyDebug("No receipt for " + item.definition.id.ToString());
        }
    }
    

    public string GetProductPrice(string productId)
    {
        if (IsInitialized())
        {
            UnityEngine.Purchasing.Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                return product.metadata.localizedPriceString;
            }
        }

        return "...";
    }

    public bool HasRecipe(string id, out bool isInit)
    {
        isInit = false;
        if (IsInitialized())
        {
            UnityEngine.Purchasing.Product product = m_StoreController.products.WithID(id);
            if (product != null && product.availableToPurchase)
            {
                isInit = true;
                return product.hasReceipt;
            }
        }

        return false;
    }

    void ProductDetailsFailedFunction(int myInt)
    {
        MyDebug("Listener = " + myInt.ToString());
    }
    private void MyDebug(string debug)
    {
        Debug.Log(debug);
    }

}
